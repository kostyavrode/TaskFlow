namespace TaskFlow.Infrastructure.Idempotency;

public interface IIdempotencyService
{
    Task<bool> IsProcessedAsync(Guid eventId, string consumerName, CancellationToken cancellationToken = default);
    Task MarkAsProcessedAsync(Guid eventId, string consumerName, CancellationToken cancellationToken = default);
}

