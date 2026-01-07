using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Notification.Application.Hubs;
using Notification.Application.Models;

namespace Notification.Application.Services;

public class NotificationService : INotificationService
{
    private readonly IHubContext<TaskNotificationHub, ITaskNotificationClient> _hubContext;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IHubContext<TaskNotificationHub, ITaskNotificationClient> hubContext,
        ILogger<NotificationService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task NotifyUserAsync(string userId, TaskNotification notification, CancellationToken cancellationToken = default)
    {
        var groupName = $"user-{userId}";
        
        await SendToGroupAsync(groupName, notification, cancellationToken);
        
        _logger.LogInformation("Sent {EventType} notification to user {UserId} for task {TaskId}", 
            notification.EventType, userId, notification.TaskId);
    }

    public async Task NotifyTaskSubscribersAsync(Guid taskId, TaskNotification notification, CancellationToken cancellationToken = default)
    {
        var groupName = $"task-{taskId}";
        
        await SendToGroupAsync(groupName, notification, cancellationToken);
        
        _logger.LogInformation("Sent {EventType} notification to task subscribers for task {TaskId}", 
            notification.EventType, taskId);
    }

    public async Task BroadcastAsync(TaskNotification notification, CancellationToken cancellationToken = default)
    {
        await _hubContext.Clients.All.ReceiveNotification(notification);
        
        _logger.LogInformation("Broadcast {EventType} notification for task {TaskId}", 
            notification.EventType, notification.TaskId);
    }

    private async Task SendToGroupAsync(string groupName, TaskNotification notification, CancellationToken cancellationToken)
    {
        var client = _hubContext.Clients.Group(groupName);

        await client.ReceiveNotification(notification);

        var task = notification.EventType switch
        {
            NotificationEventTypes.TaskCreated => client.TaskCreated(notification),
            NotificationEventTypes.TaskStarted => client.TaskStarted(notification),
            NotificationEventTypes.TaskProgress => client.TaskProgress(notification),
            NotificationEventTypes.TaskCompleted => client.TaskCompleted(notification),
            NotificationEventTypes.TaskFailed => client.TaskFailed(notification),
            NotificationEventTypes.TaskCancelled => client.TaskCancelled(notification),
            _ => Task.CompletedTask
        };

        await task;
    }
}





