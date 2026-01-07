using Microsoft.EntityFrameworkCore;
using TaskExecution.Domain.Entities;
using TaskExecution.Domain.Repositories;

namespace TaskExecution.Infrastructure.Persistence;

public class ExecutionRepository : IExecutionRepository
{
    private readonly TaskExecutionDbContext _context;

    public ExecutionRepository(TaskExecutionDbContext context)
    {
        _context = context;
    }

    public async Task<ExecutionRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.ExecutionRecords
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<ExecutionRecord?> GetByTaskIdAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        return await _context.ExecutionRecords
            .FirstOrDefaultAsync(e => e.TaskId == taskId, cancellationToken);
    }

    public async Task<ExecutionRecord> AddAsync(ExecutionRecord record, CancellationToken cancellationToken = default)
    {
        await _context.ExecutionRecords.AddAsync(record, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return record;
    }

    public async Task UpdateAsync(ExecutionRecord record, CancellationToken cancellationToken = default)
    {
        _context.ExecutionRecords.Update(record);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsForTaskAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
        return await _context.ExecutionRecords
            .AnyAsync(e => e.TaskId == taskId, cancellationToken);
    }
}

