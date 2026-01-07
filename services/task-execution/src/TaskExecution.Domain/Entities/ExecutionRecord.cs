using TaskExecution.Domain.Enums;

namespace TaskExecution.Domain.Entities;

public class ExecutionRecord
{
    public Guid Id { get; private set; }
    public Guid TaskId { get; private set; }
    public string UserId { get; private set; }
    public string TaskType { get; private set; }
    public string Priority { get; private set; }
    public string? Payload { get; private set; }
    public ExecutionStatus Status { get; private set; }
    public int ProgressPercent { get; private set; }
    public string? StatusMessage { get; private set; }
    public string? ResultLocation { get; private set; }
    public string? ErrorMessage { get; private set; }
    public int RetryCount { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string CorrelationId { get; private set; }

    private ExecutionRecord() 
    {
        UserId = string.Empty;
        TaskType = string.Empty;
        Priority = string.Empty;
        CorrelationId = string.Empty;
    }

    public static ExecutionRecord Create(
        Guid taskId,
        string userId,
        string taskType,
        string priority,
        string? payload,
        string correlationId)
    {
        return new ExecutionRecord
        {
            Id = Guid.NewGuid(),
            TaskId = taskId,
            UserId = userId,
            TaskType = taskType,
            Priority = priority,
            Payload = payload,
            Status = ExecutionStatus.Queued,
            ProgressPercent = 0,
            RetryCount = 0,
            CreatedAt = DateTime.UtcNow,
            CorrelationId = correlationId
        };
    }

    public void Start()
    {
        if (Status != ExecutionStatus.Queued)
            throw new InvalidOperationException($"Cannot start execution in {Status} status");

        Status = ExecutionStatus.Running;
        StartedAt = DateTime.UtcNow;
        ProgressPercent = 0;
    }

    public void UpdateProgress(int percent, string? message = null)
    {
        if (Status != ExecutionStatus.Running)
            throw new InvalidOperationException($"Cannot update progress in {Status} status");

        ProgressPercent = Math.Clamp(percent, 0, 100);
        StatusMessage = message;
    }

    public void Complete(string? resultLocation = null)
    {
        if (Status != ExecutionStatus.Running)
            throw new InvalidOperationException($"Cannot complete execution in {Status} status");

        Status = ExecutionStatus.Completed;
        ProgressPercent = 100;
        ResultLocation = resultLocation;
        CompletedAt = DateTime.UtcNow;
    }

    public void Fail(string errorMessage)
    {
        Status = ExecutionStatus.Failed;
        ErrorMessage = errorMessage;
        CompletedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == ExecutionStatus.Completed)
            throw new InvalidOperationException("Cannot cancel completed execution");

        Status = ExecutionStatus.Cancelled;
        CompletedAt = DateTime.UtcNow;
    }

    public void IncrementRetry()
    {
        RetryCount++;
    }

    public bool CanRetry(int maxRetries) => RetryCount < maxRetries && Status == ExecutionStatus.Failed;

    public void ResetForRetry()
    {
        if (!CanRetry(int.MaxValue))
            throw new InvalidOperationException("Cannot retry this execution");

        Status = ExecutionStatus.Queued;
        StartedAt = null;
        CompletedAt = null;
        ErrorMessage = null;
        ProgressPercent = 0;
    }
}

