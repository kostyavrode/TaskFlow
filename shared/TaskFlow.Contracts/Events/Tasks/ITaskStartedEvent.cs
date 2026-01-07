namespace TaskFlow.Contracts.Events.Tasks;

public interface ITaskStartedEvent : IEvent
{
    Guid TaskId { get; }
    string UserId { get; }
    DateTime StartedAt { get; }
}

