using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Events;
using TaskManagement.Domain.Repositories;
using TaskManagement.Domain.Services;
using TaskManagement.Domain.ValueObjects;

namespace TaskManagement.Application.Tasks.Commands.CreateTask;

public class CreateTaskHandler : IRequestHandler<CreateTaskCommand, Result<CreateTaskResponse>>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IEventPublisher _eventPublisher;
    private const int MaxUserTasks = 100;

    public CreateTaskHandler(ITaskRepository taskRepository, IEventPublisher eventPublisher)
    {
        _taskRepository = taskRepository;
        _eventPublisher = eventPublisher;
    }

    public async Task<Result<CreateTaskResponse>> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        var userTaskCount = await _taskRepository.GetUserTaskCountAsync(request.UserId, cancellationToken);
        if (userTaskCount >= MaxUserTasks)
        {
            return Result<CreateTaskResponse>.Failure($"User has reached maximum task limit ({MaxUserTasks})");
        }

        var taskType = TaskType.FromString(request.TaskType);
        var priority = Priority.FromString(request.Priority);

        var task = new TaskEntity(
            request.UserId,
            taskType,
            priority,
            request.Payload,
            request.ScheduledAt
        );

        task.MarkAsPending();

        await _taskRepository.AddAsync(task, cancellationToken);

        var taskCreatedEvent = new TaskCreatedEvent(
            task.Id,
            task.UserId,
            task.Type.Name,
            task.Priority.Name,
            task.Payload,
            task.ScheduledAt
        );

        await _eventPublisher.PublishAsync(taskCreatedEvent, cancellationToken);

        return Result<CreateTaskResponse>.Success(new CreateTaskResponse
        {
            TaskId = task.Id,
            Status = task.Status.ToString(),
            CreatedAt = task.CreatedAt
        });
    }
}


