using MediatR;
using TaskManagement.Application.Common;

namespace TaskManagement.Application.Tasks.Commands.CancelTask;

public record CancelTaskCommand : IRequest<Result>
{
    public Guid TaskId { get; init; }
    public string UserId { get; init; } = string.Empty;
}


