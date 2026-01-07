using TaskManagement.Domain.Common;

namespace TaskManagement.Domain.Events;

public record TaskCancelledEvent : IDomainEvent
{
    public Guid EventId { get; }
    public DateTime OccurredAt { get; }
    public Guid TaskId { get; }
    public string UserId { get; }

    public TaskCancelledEvent(Guid taskId, string userId)
    {
        EventId = Guid.NewGuid();
        OccurredAt = DateTime.UtcNow;
        TaskId = taskId;
        UserId = userId;
    }
}


