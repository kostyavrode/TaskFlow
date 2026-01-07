using TaskManagement.Domain.Common;

namespace TaskManagement.Domain.Events;

public record TaskPriorityChangedEvent : IDomainEvent
{
    public Guid EventId { get; }
    public DateTime OccurredAt { get; }
    public string CorrelationId { get; }
    public Guid TaskId { get; }
    public string OldPriority { get; }
    public string NewPriority { get; }

    public TaskPriorityChangedEvent(Guid taskId, string oldPriority, string newPriority, string? correlationId = null)
    {
        EventId = Guid.NewGuid();
        OccurredAt = DateTime.UtcNow;
        CorrelationId = correlationId ?? Guid.NewGuid().ToString();
        TaskId = taskId;
        OldPriority = oldPriority;
        NewPriority = newPriority;
    }
}
