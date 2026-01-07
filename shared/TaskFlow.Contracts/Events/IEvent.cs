namespace TaskFlow.Contracts.Events;

public interface IEvent
{
    Guid EventId { get; }
    DateTime OccurredAt { get; }
    string CorrelationId { get; }
}

