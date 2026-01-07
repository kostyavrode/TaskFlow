using MediatR;
using TaskManagement.Application.Common;

namespace TaskManagement.Application.Tasks.Queries.GetTask;

public record GetTaskQuery : IRequest<Result<TaskDto>>
{
    public Guid TaskId { get; init; }
    public string UserId { get; init; } = string.Empty;
}


