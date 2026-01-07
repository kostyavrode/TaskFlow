namespace TaskFlow.Contracts.Events.Tasks;

public interface ITaskCancelledEvent : IEvent
{
    Guid TaskId { get; }
    string UserId { get; }
    DateTime CancelledAt { get; }
}

