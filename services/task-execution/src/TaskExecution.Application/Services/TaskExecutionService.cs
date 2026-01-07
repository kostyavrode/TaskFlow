using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TaskExecution.Domain.Entities;
using TaskExecution.Domain.Events;
using TaskExecution.Domain.Repositories;
using TaskExecution.Domain.Services;

namespace TaskExecution.Application.Services;

public class TaskExecutionService
{
    private readonly IExecutionRepository _repository;
    private readonly ITaskExecutor _executor;
    private readonly IEventPublisher _eventPublisher;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<TaskExecutionService> _logger;
    private const int MaxRetries = 3;

    public TaskExecutionService(
        IExecutionRepository repository,
        ITaskExecutor executor,
        IEventPublisher eventPublisher,
        IServiceScopeFactory scopeFactory,
        ILogger<TaskExecutionService> logger)
    {
        _repository = repository;
        _executor = executor;
        _eventPublisher = eventPublisher;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task<bool> ProcessTaskAsync(
        Guid taskId,
        string userId,
        string taskType,
        string priority,
        string? payload,
        string correlationId,
        CancellationToken cancellationToken)
    {
        var existingRecord = await _repository.GetByTaskIdAsync(taskId, cancellationToken);
        
        ExecutionRecord record;
        if (existingRecord != null)
        {
            if (!existingRecord.CanRetry(MaxRetries))
            {
                _logger.LogWarning("Task {TaskId} already processed and cannot retry", taskId);
                return false;
            }
            
            existingRecord.ResetForRetry();
            existingRecord.IncrementRetry();
            record = existingRecord;
        }
        else
        {
            record = ExecutionRecord.Create(taskId, userId, taskType, priority, payload, correlationId);
            await _repository.AddAsync(record, cancellationToken);
        }

        return await ExecuteWithRetryAsync(record, cancellationToken);
    }

    private async Task<bool> ExecuteWithRetryAsync(ExecutionRecord record, CancellationToken cancellationToken)
    {
        try
        {
            record.Start();
            await _repository.UpdateAsync(record, cancellationToken);

            await _eventPublisher.PublishAsync(
                new TaskStartedEvent(record.TaskId, record.UserId, record.StartedAt!.Value, record.CorrelationId),
                cancellationToken);

            var taskId = record.TaskId;
            var userId = record.UserId;
            var correlationId = record.CorrelationId;

            var progress = new Progress<ExecutionProgress>(async p =>
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var repository = scope.ServiceProvider.GetRequiredService<IExecutionRepository>();
                    var eventPublisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();

                    var currentRecord = await repository.GetByTaskIdAsync(taskId, CancellationToken.None);
                    if (currentRecord == null)
                    {
                        _logger.LogWarning("Cannot update progress - record not found for task {TaskId}", taskId);
                        return;
                    }

                    currentRecord.UpdateProgress(p.Percent, p.Message);
                    await repository.UpdateAsync(currentRecord, CancellationToken.None);

                    await eventPublisher.PublishAsync(
                        new TaskProgressUpdatedEvent(taskId, userId, p.Percent, p.Message, correlationId),
                        CancellationToken.None);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to update progress for task {TaskId}", taskId);
                }
            });

            var result = await _executor.ExecuteAsync(record, progress, cancellationToken);

            if (result.Success)
            {
                record.Complete(result.ResultLocation);
                await _repository.UpdateAsync(record, cancellationToken);

                await _eventPublisher.PublishAsync(
                    new TaskCompletedEvent(record.TaskId, record.UserId, result.ResultLocation, record.CompletedAt!.Value, record.CorrelationId),
                    cancellationToken);

                _logger.LogInformation("Task {TaskId} completed successfully", record.TaskId);
                return true;
            }
            else
            {
                return await HandleFailureAsync(record, result.ErrorMessage ?? "Unknown error", null, cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            record.Cancel();
            await _repository.UpdateAsync(record, CancellationToken.None);
            _logger.LogWarning("Task {TaskId} was cancelled", record.TaskId);
            return false;
        }
        catch (Exception ex)
        {
            return await HandleFailureAsync(record, ex.Message, ex.ToString(), cancellationToken);
        }
    }

    private async Task<bool> HandleFailureAsync(ExecutionRecord record, string errorMessage, string? errorDetails, CancellationToken cancellationToken)
    {
        record.Fail(errorMessage);
        await _repository.UpdateAsync(record, CancellationToken.None);

        await _eventPublisher.PublishAsync(
            new TaskFailedEvent(record.TaskId, record.UserId, errorMessage, errorDetails, record.RetryCount, record.CompletedAt!.Value, record.CorrelationId),
            CancellationToken.None);

        _logger.LogError("Task {TaskId} failed: {Error}", record.TaskId, errorMessage);

        if (record.CanRetry(MaxRetries))
        {
            _logger.LogInformation("Scheduling retry {RetryCount}/{MaxRetries} for task {TaskId}", 
                record.RetryCount + 1, MaxRetries, record.TaskId);
            return false;
        }

        return false;
    }

    public async Task HandleTaskCancelledAsync(Guid taskId, CancellationToken cancellationToken)
    {
        var record = await _repository.GetByTaskIdAsync(taskId, cancellationToken);
        if (record == null) return;

        try
        {
            record.Cancel();
            await _repository.UpdateAsync(record, cancellationToken);
            _logger.LogInformation("Task {TaskId} execution cancelled", taskId);
        }
        catch (InvalidOperationException)
        {
            _logger.LogWarning("Could not cancel task {TaskId} - already completed", taskId);
        }
    }
}
