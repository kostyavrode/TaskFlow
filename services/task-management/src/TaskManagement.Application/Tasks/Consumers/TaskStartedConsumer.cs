using MassTransit;
using Microsoft.Extensions.Logging;
using TaskManagement.Domain.Repositories;
using TaskFlow.Contracts.Events.Tasks;

namespace TaskManagement.Application.Tasks.Consumers;

public class TaskStartedConsumer : IConsumer<ITaskStartedEvent>
{
    private readonly ITaskRepository _taskRepository;
    private readonly ILogger<TaskStartedConsumer> _logger;

    public TaskStartedConsumer(ITaskRepository taskRepository, ILogger<TaskStartedConsumer> logger)
    {
        _taskRepository = taskRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ITaskStartedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Received TaskStarted event for task {TaskId}", message.TaskId);

        var task = await _taskRepository.GetByIdAsync(message.TaskId, context.CancellationToken);
        if (task == null)
        {
            _logger.LogWarning("Task {TaskId} not found", message.TaskId);
            return;
        }

        try
        {
            task.MarkAsRunning();
            await _taskRepository.UpdateAsync(task, context.CancellationToken);
            _logger.LogInformation("Task {TaskId} marked as running", message.TaskId);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Could not mark task {TaskId} as running: {Error}", message.TaskId, ex.Message);
        }
    }
}





