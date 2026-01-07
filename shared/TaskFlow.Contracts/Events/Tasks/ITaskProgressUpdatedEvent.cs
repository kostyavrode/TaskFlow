namespace TaskFlow.Contracts.Events.Tasks;

public interface ITaskProgressUpdatedEvent : IEvent
{
    Guid TaskId { get; }
    string UserId { get; }
    int ProgressPercent { get; }
    string? StatusMessage { get; }
}

