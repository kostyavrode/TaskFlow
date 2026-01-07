using TaskManagement.Domain.Common;
using TaskManagement.Domain.Enums;
using TaskManagement.Domain.ValueObjects;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Domain.Entities;

public class TaskEntity : BaseEntity
{
    public string UserId { get; private set; }
    public TaskType Type { get; private set; }
    public Priority Priority { get; private set; }
    public string? Payload { get; private set; }
    public TaskStatus Status { get; private set; }
    public DateTime? ScheduledAt { get; private set; }
    public string? ResultLocation { get; private set; }

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private TaskEntity() { }

    public TaskEntity(string userId, TaskType type, Priority priority, string? payload, DateTime? scheduledAt = null)
    {
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        Type = type ?? throw new ArgumentNullException(nameof(type));
        Priority = priority ?? throw new ArgumentNullException(nameof(priority));
        Payload = payload;
        Status = TaskStatus.Created;
        ScheduledAt = scheduledAt;
    }

    public void MarkAsPending()
    {
        if (Status != TaskStatus.Created)
            throw new InvalidOperationException($"Cannot mark task as pending. Current status: {Status}");

        Status = TaskStatus.Pending;
        SetUpdatedAt();
    }

    public void Cancel()
    {
        if (Status == TaskStatus.Cancelled)
            throw new InvalidOperationException("Task is already cancelled");

        Status = TaskStatus.Cancelled;
        SetUpdatedAt();
    }

    public void MarkAsRunning()
    {
        if (Status != TaskStatus.Pending)
            throw new InvalidOperationException($"Cannot mark task as running. Current status: {Status}");

        Status = TaskStatus.Running;
        SetUpdatedAt();
    }

    public void MarkAsCompleted(string? resultLocation = null)
    {
        if (Status != TaskStatus.Running && Status != TaskStatus.Pending)
            throw new InvalidOperationException($"Cannot mark task as completed. Current status: {Status}");

        Status = TaskStatus.Completed;
        ResultLocation = resultLocation;
        SetUpdatedAt();
    }

    public void MarkAsFailed()
    {
        if (Status == TaskStatus.Cancelled || Status == TaskStatus.Completed)
            throw new InvalidOperationException($"Cannot mark task as failed. Current status: {Status}");

        Status = TaskStatus.Failed;
        SetUpdatedAt();
    }

    public void ChangePriority(Priority newPriority)
    {
        if (newPriority == null)
            throw new ArgumentNullException(nameof(newPriority));

        if (Status == TaskStatus.Cancelled)
            throw new InvalidOperationException("Cannot change priority of cancelled task");

        Priority = newPriority;
        SetUpdatedAt();
    }

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}

