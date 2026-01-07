using Microsoft.EntityFrameworkCore;
using TaskFlow.Infrastructure.Idempotency;

namespace TaskExecution.Infrastructure.Persistence;

public class IdempotencyService : IIdempotencyService
{
    private readonly TaskExecutionDbContext _context;

    public IdempotencyService(TaskExecutionDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsProcessedAsync(Guid eventId, string consumerName, CancellationToken cancellationToken = default)
    {
        return await _context.ProcessedEvents
            .AnyAsync(e => e.EventId == eventId && e.ConsumerName == consumerName, cancellationToken);
    }

    public async Task MarkAsProcessedAsync(Guid eventId, string consumerName, CancellationToken cancellationToken = default)
    {
        var processedEvent = new ProcessedEvent
        {
            EventId = eventId,
            ConsumerName = consumerName,
            ProcessedAt = DateTime.UtcNow
        };

        await _context.ProcessedEvents.AddAsync(processedEvent, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

