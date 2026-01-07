using MediatR;
using TaskManagement.Application.Common;
using TaskManagement.Application.Tasks.Queries.GetTask;

namespace TaskManagement.Application.Tasks.Queries.GetUserTasks;

public record GetUserTasksQuery : IRequest<Result<List<TaskDto>>>
{
    public string UserId { get; init; } = string.Empty;
}


