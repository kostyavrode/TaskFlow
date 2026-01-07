using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Domain.Events;
using TaskManagement.Domain.Repositories;
using TaskManagement.Domain.Services;

namespace TaskManagement.Application.Tasks.Commands.CancelTask;

public class CancelTaskHandler : IRequestHandler<CancelTaskCommand, Result>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IEventPublisher _eventPublisher;

    public CancelTaskHandler(ITaskRepository taskRepository, IEventPublisher eventPublisher)
    {
        _taskRepository = taskRepository;
        _eventPublisher = eventPublisher;
    }

    public async Task<Result> Handle(CancelTaskCommand request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(request.TaskId, cancellationToken);
        if (task == null)
        {
            return Result.Failure("Task not found");
        }

        if (task.UserId != request.UserId)
        {
            return Result.Failure("Unauthorized: task belongs to different user");
        }

        try
        {
            task.Cancel();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }

        await _taskRepository.UpdateAsync(task, cancellationToken);

        var taskCancelledEvent = new TaskCancelledEvent(task.Id, task.UserId);
        await _eventPublisher.PublishAsync(taskCancelledEvent, cancellationToken);

        return Result.Success();
    }
}


