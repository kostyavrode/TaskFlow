using TaskManagement.Domain.Common;

namespace TaskManagement.Domain.Events;

public record TaskCreatedEvent : IDomainEvent
{
    public Guid EventId { get; }
    public DateTime OccurredAt { get; }
    public Guid TaskId { get; }
    public string UserId { get; }
    public string TaskType { get; }
    public string Priority { get; }
    public string? Payload { get; }
    public DateTime? ScheduledAt { get; }

    public TaskCreatedEvent(
        Guid taskId,
        string userId,
        string taskType,
        string priority,
        string? payload,
        DateTime? scheduledAt)
    {
        EventId = Guid.NewGuid();
        OccurredAt = DateTime.UtcNow;
        TaskId = taskId;
        UserId = userId;
        TaskType = taskType;
        Priority = priority;
        Payload = payload;
        ScheduledAt = scheduledAt;
    }
}


