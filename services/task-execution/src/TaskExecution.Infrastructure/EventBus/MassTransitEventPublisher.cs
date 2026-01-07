using MassTransit;
using TaskExecution.Application.Services;
using TaskFlow.Contracts.Events;

namespace TaskExecution.Infrastructure.EventBus;

public class MassTransitEventPublisher : IEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;

    public MassTransitEventPublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class, IEvent
    {
        await _publishEndpoint.Publish(@event, cancellationToken);
    }
}

