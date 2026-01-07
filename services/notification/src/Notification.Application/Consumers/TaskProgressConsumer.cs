using MassTransit;
using Microsoft.Extensions.Logging;
using Notification.Application.Models;
using Notification.Application.Services;
using TaskFlow.Contracts.Events.Tasks;

namespace Notification.Application.Consumers;

public class TaskProgressConsumer : IConsumer<ITaskProgressUpdatedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<TaskProgressConsumer> _logger;

    public TaskProgressConsumer(INotificationService notificationService, ILogger<TaskProgressConsumer> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ITaskProgressUpdatedEvent> context)
    {
        var message = context.Message;
        _logger.LogDebug("Received TaskProgress event for task {TaskId}: {Percent}%", message.TaskId, message.ProgressPercent);

        var notification = new TaskNotification
        {
            TaskId = message.TaskId,
            UserId = message.UserId,
            EventType = NotificationEventTypes.TaskProgress,
            Status = "Running",
            ProgressPercent = message.ProgressPercent,
            Message = message.StatusMessage ?? $"Progress: {message.ProgressPercent}%",
            Timestamp = message.OccurredAt
        };

        await _notificationService.NotifyUserAsync(message.UserId, notification, context.CancellationToken);
        await _notificationService.NotifyTaskSubscribersAsync(message.TaskId, notification, context.CancellationToken);
    }
}

