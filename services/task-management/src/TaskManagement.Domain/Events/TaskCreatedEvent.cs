using TaskFlow.Contracts.Events.Tasks;
using TaskManagement.Domain.Common;

namespace TaskManagement.Domain.Events;

public record TaskCreatedEvent : IDomainEvent, ITaskCreatedEvent
{
    public Guid EventId { get; }
    public DateTime OccurredAt { get; }
    public string CorrelationId { get; }
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
        DateTime? scheduledAt,
        string? correlationId = null)
    {
        EventId = Guid.NewGuid();
        OccurredAt = DateTime.UtcNow;
        CorrelationId = correlationId ?? Guid.NewGuid().ToString();
        TaskId = taskId;
        UserId = userId;
        TaskType = taskType;
        Priority = priority;
        Payload = payload;
        ScheduledAt = scheduledAt;
    }
}
