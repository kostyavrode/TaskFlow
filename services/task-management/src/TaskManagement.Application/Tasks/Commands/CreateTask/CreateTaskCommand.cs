using MediatR;
using TaskManagement.Application.Common;

namespace TaskManagement.Application.Tasks.Commands.CreateTask;

public record CreateTaskCommand : IRequest<Result<CreateTaskResponse>>
{
    public string UserId { get; init; } = string.Empty;
    public string TaskType { get; init; } = string.Empty;
    public string Priority { get; init; } = string.Empty;
    public string? Payload { get; init; }
    public DateTime? ScheduledAt { get; init; }
}


