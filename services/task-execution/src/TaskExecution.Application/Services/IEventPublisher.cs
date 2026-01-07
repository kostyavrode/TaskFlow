using TaskFlow.Contracts.Events;

namespace TaskExecution.Application.Services;

public interface IEventPublisher
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class, IEvent;
}

