namespace Notification.Application.Models;

public record TaskNotification
{
    public Guid TaskId { get; init; }
    public string UserId { get; init; } = string.Empty;
    public string EventType { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public int? ProgressPercent { get; init; }
    public string? Message { get; init; }
    public string? ResultLocation { get; init; }
    public string? ErrorMessage { get; init; }
    public DateTime Timestamp { get; init; }
}

public static class NotificationEventTypes
{
    public const string TaskCreated = "TaskCreated";
    public const string TaskStarted = "TaskStarted";
    public const string TaskProgress = "TaskProgress";
    public const string TaskCompleted = "TaskCompleted";
    public const string TaskFailed = "TaskFailed";
    public const string TaskCancelled = "TaskCancelled";
}






