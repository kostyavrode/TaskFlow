using MassTransit;
using Microsoft.Extensions.Logging;
using Notification.Application.Models;
using Notification.Application.Services;
using TaskFlow.Contracts.Events.Tasks;

namespace Notification.Application.Consumers;

public class TaskStartedConsumer : IConsumer<ITaskStartedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<TaskStartedConsumer> _logger;

    public TaskStartedConsumer(INotificationService notificationService, ILogger<TaskStartedConsumer> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ITaskStartedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Received TaskStarted event for task {TaskId}", message.TaskId);

        var notification = new TaskNotification
        {
            TaskId = message.TaskId,
            UserId = message.UserId,
            EventType = NotificationEventTypes.TaskStarted,
            Status = "Running",
            ProgressPercent = 0,
            Message = "Task execution started",
            Timestamp = message.StartedAt
        };

        await _notificationService.NotifyUserAsync(message.UserId, notification, context.CancellationToken);
        await _notificationService.NotifyTaskSubscribersAsync(message.TaskId, notification, context.CancellationToken);
    }
}

