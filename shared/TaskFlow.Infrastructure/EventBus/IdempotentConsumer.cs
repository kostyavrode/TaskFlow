using MassTransit;
using Microsoft.Extensions.Logging;
using TaskFlow.Contracts.Events;
using TaskFlow.Infrastructure.Idempotency;

namespace TaskFlow.Infrastructure.EventBus;

public abstract class IdempotentConsumer<TEvent> : IConsumer<TEvent>
    where TEvent : class, IEvent
{
    private readonly IIdempotencyService _idempotencyService;
    private readonly ILogger _logger;

    protected IdempotentConsumer(IIdempotencyService idempotencyService, ILogger logger)
    {
        _idempotencyService = idempotencyService;
        _logger = logger;
    }

    protected abstract string ConsumerName { get; }

    public async Task Consume(ConsumeContext<TEvent> context)
    {
        var eventId = context.Message.EventId;
        var correlationId = context.Message.CorrelationId;

        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["EventId"] = eventId,
            ["CorrelationId"] = correlationId,
            ["Consumer"] = ConsumerName
        });

        if (await _idempotencyService.IsProcessedAsync(eventId, ConsumerName, context.CancellationToken))
        {
            _logger.LogInformation("Event {EventId} already processed by {Consumer}, skipping", eventId, ConsumerName);
            return;
        }

        try
        {
            _logger.LogInformation("Processing event {EventId}", eventId);
            
            await ProcessEventAsync(context.Message, context.CancellationToken);
            
            await _idempotencyService.MarkAsProcessedAsync(eventId, ConsumerName, context.CancellationToken);
            
            _logger.LogInformation("Successfully processed event {EventId}", eventId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process event {EventId}", eventId);
            throw;
        }
    }

    protected abstract Task ProcessEventAsync(TEvent @event, CancellationToken cancellationToken);
}

