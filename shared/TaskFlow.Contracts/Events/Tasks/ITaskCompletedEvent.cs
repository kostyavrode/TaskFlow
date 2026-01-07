namespace TaskFlow.Contracts.Events.Tasks;

public interface ITaskCompletedEvent : IEvent
{
    Guid TaskId { get; }
    string UserId { get; }
    string? ResultLocation { get; }
    DateTime CompletedAt { get; }
}

