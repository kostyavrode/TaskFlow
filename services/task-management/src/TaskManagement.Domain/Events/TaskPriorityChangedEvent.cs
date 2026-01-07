using TaskManagement.Domain.Common;

namespace TaskManagement.Domain.Events;

public record TaskPriorityChangedEvent : IDomainEvent
{
    public Guid EventId { get; }
    public DateTime OccurredAt { get; }
    public Guid TaskId { get; }
    public string OldPriority { get; }
    public string NewPriority { get; }

    public TaskPriorityChangedEvent(Guid taskId, string oldPriority, string newPriority)
    {
        EventId = Guid.NewGuid();
        OccurredAt = DateTime.UtcNow;
        TaskId = taskId;
        OldPriority = oldPriority;
        NewPriority = newPriority;
    }
}


