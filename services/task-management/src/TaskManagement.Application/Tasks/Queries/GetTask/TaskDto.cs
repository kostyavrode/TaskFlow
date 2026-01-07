namespace TaskManagement.Application.Tasks.Queries.GetTask;

public record TaskDto
{
    public Guid Id { get; init; }
    public string UserId { get; init; } = string.Empty;
    public string TaskType { get; init; } = string.Empty;
    public string Priority { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string? Payload { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public DateTime? ScheduledAt { get; init; }
}


