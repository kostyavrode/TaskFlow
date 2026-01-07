namespace TaskManagement.Application.Tasks.Commands.CreateTask;

public record CreateTaskResponse
{
    public Guid TaskId { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}


