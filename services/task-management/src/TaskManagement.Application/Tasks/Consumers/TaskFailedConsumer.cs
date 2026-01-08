using MassTransit;
using Microsoft.Extensions.Logging;
using TaskManagement.Domain.Repositories;
using TaskFlow.Contracts.Events.Tasks;

namespace TaskManagement.Application.Tasks.Consumers;

public class TaskFailedConsumer : IConsumer<ITaskFailedEvent>
{
    private readonly ITaskRepository _taskRepository;
    private readonly ILogger<TaskFailedConsumer> _logger;

    public TaskFailedConsumer(ITaskRepository taskRepository, ILogger<TaskFailedConsumer> logger)
    {
        _taskRepository = taskRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ITaskFailedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Received TaskFailed event for task {TaskId}", message.TaskId);

        var task = await _taskRepository.GetByIdAsync(message.TaskId, context.CancellationToken);
        if (task == null)
        {
            _logger.LogWarning("Task {TaskId} not found", message.TaskId);
            return;
        }

        try
        {
            task.MarkAsFailed();
            await _taskRepository.UpdateAsync(task, context.CancellationToken);
            _logger.LogInformation("Task {TaskId} marked as failed", message.TaskId);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Could not mark task {TaskId} as failed: {Error}", message.TaskId, ex.Message);
        }
    }
}






