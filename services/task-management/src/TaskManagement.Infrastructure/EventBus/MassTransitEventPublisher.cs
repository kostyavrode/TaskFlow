using MassTransit;
using TaskManagement.Domain.Common;
using TaskManagement.Domain.Services;

namespace TaskManagement.Infrastructure.EventBus;

public class MassTransitEventPublisher : IEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;

    public MassTransitEventPublisher(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) 
        where TEvent : IDomainEvent
    {
        await _publishEndpoint.Publish(@event, cancellationToken);
    }

    public async Task PublishBatchAsync<TEvent>(IEnumerable<TEvent> events, CancellationToken cancellationToken = default) 
        where TEvent : class, IDomainEvent
    {
        await _publishEndpoint.PublishBatch(events, cancellationToken);
    }
}

