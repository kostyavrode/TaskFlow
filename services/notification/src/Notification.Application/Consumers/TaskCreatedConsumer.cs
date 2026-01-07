using MassTransit;
using Microsoft.Extensions.Logging;
using Notification.Application.Models;
using Notification.Application.Services;
using TaskFlow.Contracts.Events.Tasks;

namespace Notification.Application.Consumers;

public class TaskCreatedConsumer : IConsumer<ITaskCreatedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<TaskCreatedConsumer> _logger;

    public TaskCreatedConsumer(INotificationService notificationService, ILogger<TaskCreatedConsumer> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ITaskCreatedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Received TaskCreated event for task {TaskId}", message.TaskId);

        var notification = new TaskNotification
        {
            TaskId = message.TaskId,
            UserId = message.UserId,
            EventType = NotificationEventTypes.TaskCreated,
            Status = "Created",
            Message = $"Task {message.TaskType} created with priority {message.Priority}",
            Timestamp = message.OccurredAt
        };

        await _notificationService.NotifyUserAsync(message.UserId, notification, context.CancellationToken);
    }
}





