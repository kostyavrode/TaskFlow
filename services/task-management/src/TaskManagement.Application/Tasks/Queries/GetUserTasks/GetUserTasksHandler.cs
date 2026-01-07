using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.Tasks.Queries.GetTask;
using TaskManagement.Domain.Repositories;

namespace TaskManagement.Application.Tasks.Queries.GetUserTasks;

public class GetUserTasksHandler : IRequestHandler<GetUserTasksQuery, Result<List<TaskDto>>>
{
    private readonly ITaskRepository _taskRepository;

    public GetUserTasksHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<Result<List<TaskDto>>> Handle(GetUserTasksQuery request, CancellationToken cancellationToken)
    {
        var tasks = await _taskRepository.GetByUserIdAsync(request.UserId, cancellationToken);

        var taskDtos = tasks.Select(task => new TaskDto
        {
            Id = task.Id,
            UserId = task.UserId,
            TaskType = task.Type.Name,
            Priority = task.Priority.Name,
            Status = task.Status.ToString(),
            Payload = task.Payload,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt,
            ScheduledAt = task.ScheduledAt
        }).ToList();

        return Result<List<TaskDto>>.Success(taskDtos);
    }
}


