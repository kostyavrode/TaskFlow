using MassTransit;
using Microsoft.Extensions.Logging;
using TaskExecution.Application.Services;
using TaskFlow.Contracts.Events.Tasks;

namespace TaskExecution.Application.Consumers;

public class TaskCancelledConsumer : IConsumer<ITaskCancelledEvent>
{
    private readonly TaskExecutionService _executionService;
    private readonly ILogger<TaskCancelledConsumer> _logger;

    public TaskCancelledConsumer(TaskExecutionService executionService, ILogger<TaskCancelledConsumer> logger)
    {
        _executionService = executionService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ITaskCancelledEvent> context)
    {
        var message = context.Message;

        _logger.LogInformation("Received TaskCancelled event for task {TaskId}", message.TaskId);

        await _executionService.HandleTaskCancelledAsync(message.TaskId, context.CancellationToken);
    }
}

