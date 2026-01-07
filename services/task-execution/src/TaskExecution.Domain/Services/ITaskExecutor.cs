using TaskExecution.Domain.Entities;

namespace TaskExecution.Domain.Services;

public interface ITaskExecutor
{
    Task<ExecutionResult> ExecuteAsync(ExecutionRecord record, IProgress<ExecutionProgress> progress, CancellationToken cancellationToken);
}

public record ExecutionResult(bool Success, string? ResultLocation = null, string? ErrorMessage = null);

public record ExecutionProgress(int Percent, string? Message = null);

