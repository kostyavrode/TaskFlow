using TaskManagement.Domain.Entities;

namespace TaskManagement.Domain.Repositories;

public interface ITaskRepository
{
    Task<TaskEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TaskEntity>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<TaskEntity> AddAsync(TaskEntity task, CancellationToken cancellationToken = default);
    Task UpdateAsync(TaskEntity task, CancellationToken cancellationToken = default);
    Task<int> GetUserTaskCountAsync(string userId, CancellationToken cancellationToken = default);
}


