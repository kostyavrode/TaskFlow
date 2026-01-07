using Microsoft.Extensions.Logging;
using TaskExecution.Domain.Entities;
using TaskExecution.Domain.Services;

namespace TaskExecution.Infrastructure.Executors;

public class DefaultTaskExecutor : ITaskExecutor
{
    private readonly ILogger<DefaultTaskExecutor> _logger;
    private readonly Dictionary<string, ITaskTypeHandler> _handlers;

    public DefaultTaskExecutor(IEnumerable<ITaskTypeHandler> handlers, ILogger<DefaultTaskExecutor> logger)
    {
        _logger = logger;
        _handlers = handlers.ToDictionary(h => h.TaskType, h => h, StringComparer.OrdinalIgnoreCase);
    }

    public async Task<ExecutionResult> ExecuteAsync(
        ExecutionRecord record, 
        IProgress<ExecutionProgress> progress, 
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Executing task {TaskId} of type {TaskType}", record.TaskId, record.TaskType);

        if (_handlers.TryGetValue(record.TaskType, out var handler))
        {
            return await handler.HandleAsync(record, progress, cancellationToken);
        }

        return await ExecuteDefaultAsync(record, progress, cancellationToken);
    }

    private async Task<ExecutionResult> ExecuteDefaultAsync(
        ExecutionRecord record, 
        IProgress<ExecutionProgress> progress, 
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Using default executor for task type {TaskType}", record.TaskType);

        var steps = 5;
        var delayPerStep = GetDelayForPriority(record.Priority);

        for (int i = 1; i <= steps; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var percent = (i * 100) / steps;
            progress.Report(new ExecutionProgress(percent, $"Processing step {i}/{steps}"));

            await Task.Delay(delayPerStep, cancellationToken);
        }

        var resultLocation = $"results/{record.TaskId}/{DateTime.UtcNow:yyyyMMddHHmmss}.json";
        return new ExecutionResult(true, resultLocation);
    }

    private static TimeSpan GetDelayForPriority(string priority)
    {
        return priority.ToLowerInvariant() switch
        {
            "critical" => TimeSpan.FromMilliseconds(200),
            "high" => TimeSpan.FromMilliseconds(500),
            "medium" => TimeSpan.FromSeconds(1),
            "low" => TimeSpan.FromSeconds(2),
            _ => TimeSpan.FromSeconds(1)
        };
    }
}

public interface ITaskTypeHandler
{
    string TaskType { get; }
    Task<ExecutionResult> HandleAsync(ExecutionRecord record, IProgress<ExecutionProgress> progress, CancellationToken cancellationToken);
}

