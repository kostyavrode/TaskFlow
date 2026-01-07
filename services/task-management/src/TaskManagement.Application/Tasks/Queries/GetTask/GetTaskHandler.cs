using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Domain.Repositories;

namespace TaskManagement.Application.Tasks.Queries.GetTask;

public class GetTaskHandler : IRequestHandler<GetTaskQuery, Result<TaskDto>>
{
    private readonly ITaskRepository _taskRepository;

    public GetTaskHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<Result<TaskDto>> Handle(GetTaskQuery request, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(request.TaskId, cancellationToken);
        if (task == null)
        {
            return Result<TaskDto>.Failure("Task not found");
        }

        if (task.UserId != request.UserId)
        {
            return Result<TaskDto>.Failure("Unauthorized: task belongs to different user");
        }

        var taskDto = new TaskDto
        {
            Id = task.Id,
            UserId = task.UserId,
            TaskType = task.Type.Name,
            Priority = task.Priority.Name,
            Status = task.Status.ToString(),
            Payload = task.Payload,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt,
            ScheduledAt = task.ScheduledAt,
            ResultLocation = task.ResultLocation
        };

        return Result<TaskDto>.Success(taskDto);
    }
}


