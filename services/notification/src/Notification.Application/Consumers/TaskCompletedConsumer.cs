using MassTransit;
using Microsoft.Extensions.Logging;
using Notification.Application.Models;
using Notification.Application.Services;
using TaskFlow.Contracts.Events.Tasks;

namespace Notification.Application.Consumers;

public class TaskCompletedConsumer : IConsumer<ITaskCompletedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<TaskCompletedConsumer> _logger;

    public TaskCompletedConsumer(INotificationService notificationService, ILogger<TaskCompletedConsumer> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ITaskCompletedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Received TaskCompleted event for task {TaskId}", message.TaskId);

        var notification = new TaskNotification
        {
            TaskId = message.TaskId,
            UserId = message.UserId,
            EventType = NotificationEventTypes.TaskCompleted,
            Status = "Completed",
            ProgressPercent = 100,
            Message = "Task completed successfully",
            ResultLocation = message.ResultLocation,
            Timestamp = message.CompletedAt
        };

        await _notificationService.NotifyUserAsync(message.UserId, notification, context.CancellationToken);
        await _notificationService.NotifyTaskSubscribersAsync(message.TaskId, notification, context.CancellationToken);
    }
}

