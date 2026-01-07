using TaskExecution.Domain.Entities;

namespace TaskExecution.Domain.Repositories;

public interface IExecutionRepository
{
    Task<ExecutionRecord?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ExecutionRecord?> GetByTaskIdAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task<ExecutionRecord> AddAsync(ExecutionRecord record, CancellationToken cancellationToken = default);
    Task UpdateAsync(ExecutionRecord record, CancellationToken cancellationToken = default);
    Task<bool> ExistsForTaskAsync(Guid taskId, CancellationToken cancellationToken = default);
}

