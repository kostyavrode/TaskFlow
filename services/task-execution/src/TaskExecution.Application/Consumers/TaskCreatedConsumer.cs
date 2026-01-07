using MassTransit;
using Microsoft.Extensions.Logging;
using TaskExecution.Application.Services;
using TaskFlow.Contracts.Events.Tasks;

namespace TaskExecution.Application.Consumers;

public class TaskCreatedConsumer : IConsumer<ITaskCreatedEvent>
{
    private readonly TaskExecutionService _executionService;
    private readonly ILogger<TaskCreatedConsumer> _logger;

    public TaskCreatedConsumer(TaskExecutionService executionService, ILogger<TaskCreatedConsumer> logger)
    {
        _executionService = executionService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ITaskCreatedEvent> context)
    {
        var message = context.Message;

        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["TaskId"] = message.TaskId,
            ["CorrelationId"] = message.CorrelationId,
            ["EventId"] = message.EventId
        });

        _logger.LogInformation("Received TaskCreated event for task {TaskId}, type: {TaskType}", 
            message.TaskId, message.TaskType);

        if (message.ScheduledAt.HasValue && message.ScheduledAt.Value > DateTime.UtcNow)
        {
            _logger.LogInformation("Task {TaskId} is scheduled for {ScheduledAt}, skipping immediate execution", 
                message.TaskId, message.ScheduledAt);
            return;
        }

        await _executionService.ProcessTaskAsync(
            message.TaskId,
            message.UserId,
            message.TaskType,
            message.Priority,
            message.Payload,
            message.CorrelationId,
            context.CancellationToken);
    }
}

