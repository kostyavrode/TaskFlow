using TaskFlow.Contracts.Events.Tasks;
using TaskManagement.Domain.Common;

namespace TaskManagement.Domain.Events;

public record TaskCancelledEvent : IDomainEvent, ITaskCancelledEvent
{
    public Guid EventId { get; }
    public DateTime OccurredAt { get; }
    public string CorrelationId { get; }
    public Guid TaskId { get; }
    public string UserId { get; }
    public DateTime CancelledAt { get; }

    public TaskCancelledEvent(Guid taskId, string userId, string? correlationId = null)
    {
        EventId = Guid.NewGuid();
        OccurredAt = DateTime.UtcNow;
        CancelledAt = DateTime.UtcNow;
        CorrelationId = correlationId ?? Guid.NewGuid().ToString();
        TaskId = taskId;
        UserId = userId;
    }
}
