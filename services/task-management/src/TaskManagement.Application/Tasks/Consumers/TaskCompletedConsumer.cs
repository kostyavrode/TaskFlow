using MassTransit;
using Microsoft.Extensions.Logging;
using TaskManagement.Domain.Repositories;
using TaskFlow.Contracts.Events.Tasks;

namespace TaskManagement.Application.Tasks.Consumers;

public class TaskCompletedConsumer : IConsumer<ITaskCompletedEvent>
{
    private readonly ITaskRepository _taskRepository;
    private readonly ILogger<TaskCompletedConsumer> _logger;

    public TaskCompletedConsumer(ITaskRepository taskRepository, ILogger<TaskCompletedConsumer> logger)
    {
        _taskRepository = taskRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ITaskCompletedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Received TaskCompleted event for task {TaskId}", message.TaskId);

        var task = await _taskRepository.GetByIdAsync(message.TaskId, context.CancellationToken);
        if (task == null)
        {
            _logger.LogWarning("Task {TaskId} not found", message.TaskId);
            return;
        }

        try
        {
            task.MarkAsCompleted(message.ResultLocation);
            await _taskRepository.UpdateAsync(task, context.CancellationToken);
            _logger.LogInformation("Task {TaskId} marked as completed with result at {ResultLocation}", message.TaskId, message.ResultLocation);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Could not mark task {TaskId} as completed: {Error}", message.TaskId, ex.Message);
        }
    }
}
