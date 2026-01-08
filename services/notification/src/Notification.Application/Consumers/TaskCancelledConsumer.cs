using MassTransit;
using Microsoft.Extensions.Logging;
using Notification.Application.Models;
using Notification.Application.Services;
using TaskFlow.Contracts.Events.Tasks;

namespace Notification.Application.Consumers;

public class TaskCancelledConsumer : IConsumer<ITaskCancelledEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<TaskCancelledConsumer> _logger;

    public TaskCancelledConsumer(INotificationService notificationService, ILogger<TaskCancelledConsumer> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ITaskCancelledEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Received TaskCancelled event for task {TaskId}", message.TaskId);

        var notification = new TaskNotification
        {
            TaskId = message.TaskId,
            UserId = message.UserId,
            EventType = NotificationEventTypes.TaskCancelled,
            Status = "Cancelled",
            Message = "Task was cancelled",
            Timestamp = message.CancelledAt
        };

        await _notificationService.NotifyUserAsync(message.UserId, notification, context.CancellationToken);
        await _notificationService.NotifyTaskSubscribersAsync(message.TaskId, notification, context.CancellationToken);
    }
}






