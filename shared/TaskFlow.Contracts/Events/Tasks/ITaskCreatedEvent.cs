namespace TaskFlow.Contracts.Events.Tasks;

public interface ITaskCreatedEvent : IEvent
{
    Guid TaskId { get; }
    string UserId { get; }
    string TaskType { get; }
    string Priority { get; }
    string? Payload { get; }
    DateTime? ScheduledAt { get; }
}

