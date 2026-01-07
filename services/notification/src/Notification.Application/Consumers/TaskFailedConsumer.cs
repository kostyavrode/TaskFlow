using MassTransit;
using Microsoft.Extensions.Logging;
using Notification.Application.Models;
using Notification.Application.Services;
using TaskFlow.Contracts.Events.Tasks;

namespace Notification.Application.Consumers;

public class TaskFailedConsumer : IConsumer<ITaskFailedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<TaskFailedConsumer> _logger;

    public TaskFailedConsumer(INotificationService notificationService, ILogger<TaskFailedConsumer> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ITaskFailedEvent> context)
    {
        var message = context.Message;
        _logger.LogWarning("Received TaskFailed event for task {TaskId}: {Error}", message.TaskId, message.ErrorMessage);

        var notification = new TaskNotification
        {
            TaskId = message.TaskId,
            UserId = message.UserId,
            EventType = NotificationEventTypes.TaskFailed,
            Status = "Failed",
            Message = $"Task failed (attempt {message.RetryCount + 1})",
            ErrorMessage = message.ErrorMessage,
            Timestamp = message.FailedAt
        };

        await _notificationService.NotifyUserAsync(message.UserId, notification, context.CancellationToken);
        await _notificationService.NotifyTaskSubscribersAsync(message.TaskId, notification, context.CancellationToken);
    }
}

