namespace TaskFlow.Contracts.Events.Tasks;

public interface ITaskFailedEvent : IEvent
{
    Guid TaskId { get; }
    string UserId { get; }
    string ErrorMessage { get; }
    string? ErrorDetails { get; }
    int RetryCount { get; }
    DateTime FailedAt { get; }
}

