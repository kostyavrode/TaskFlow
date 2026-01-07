using TaskFlow.Contracts.Events;
using TaskFlow.Contracts.Events.Tasks;

namespace TaskExecution.Domain.Events;

public record TaskStartedEvent : ITaskStartedEvent
{
    public Guid EventId { get; }
    public DateTime OccurredAt { get; }
    public string CorrelationId { get; }
    public Guid TaskId { get; }
    public string UserId { get; }
    public DateTime StartedAt { get; }

    public TaskStartedEvent(Guid taskId, string userId, DateTime startedAt, string correlationId)
    {
        EventId = Guid.NewGuid();
        OccurredAt = DateTime.UtcNow;
        CorrelationId = correlationId;
        TaskId = taskId;
        UserId = userId;
        StartedAt = startedAt;
    }
}

public record TaskProgressUpdatedEvent : ITaskProgressUpdatedEvent
{
    public Guid EventId { get; }
    public DateTime OccurredAt { get; }
    public string CorrelationId { get; }
    public Guid TaskId { get; }
    public string UserId { get; }
    public int ProgressPercent { get; }
    public string? StatusMessage { get; }

    public TaskProgressUpdatedEvent(Guid taskId, string userId, int progressPercent, string? statusMessage, string correlationId)
    {
        EventId = Guid.NewGuid();
        OccurredAt = DateTime.UtcNow;
        CorrelationId = correlationId;
        TaskId = taskId;
        UserId = userId;
        ProgressPercent = progressPercent;
        StatusMessage = statusMessage;
    }
}

public record TaskCompletedEvent : ITaskCompletedEvent
{
    public Guid EventId { get; }
    public DateTime OccurredAt { get; }
    public string CorrelationId { get; }
    public Guid TaskId { get; }
    public string UserId { get; }
    public string? ResultLocation { get; }
    public DateTime CompletedAt { get; }

    public TaskCompletedEvent(Guid taskId, string userId, string? resultLocation, DateTime completedAt, string correlationId)
    {
        EventId = Guid.NewGuid();
        OccurredAt = DateTime.UtcNow;
        CorrelationId = correlationId;
        TaskId = taskId;
        UserId = userId;
        ResultLocation = resultLocation;
        CompletedAt = completedAt;
    }
}

public record TaskFailedEvent : ITaskFailedEvent
{
    public Guid EventId { get; }
    public DateTime OccurredAt { get; }
    public string CorrelationId { get; }
    public Guid TaskId { get; }
    public string UserId { get; }
    public string ErrorMessage { get; }
    public string? ErrorDetails { get; }
    public int RetryCount { get; }
    public DateTime FailedAt { get; }

    public TaskFailedEvent(Guid taskId, string userId, string errorMessage, string? errorDetails, int retryCount, DateTime failedAt, string correlationId)
    {
        EventId = Guid.NewGuid();
        OccurredAt = DateTime.UtcNow;
        CorrelationId = correlationId;
        TaskId = taskId;
        UserId = userId;
        ErrorMessage = errorMessage;
        ErrorDetails = errorDetails;
        RetryCount = retryCount;
        FailedAt = failedAt;
    }
}
