using Notification.Application.Models;

namespace Notification.Application.Services;

public interface INotificationService
{
    Task NotifyUserAsync(string userId, TaskNotification notification, CancellationToken cancellationToken = default);
    Task NotifyTaskSubscribersAsync(Guid taskId, TaskNotification notification, CancellationToken cancellationToken = default);
    Task BroadcastAsync(TaskNotification notification, CancellationToken cancellationToken = default);
}






