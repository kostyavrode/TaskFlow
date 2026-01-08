using Notification.Application.Models;

namespace Notification.Application.Hubs;

public interface ITaskNotificationClient
{
    Task ReceiveNotification(TaskNotification notification);
    Task TaskCreated(TaskNotification notification);
    Task TaskStarted(TaskNotification notification);
    Task TaskProgress(TaskNotification notification);
    Task TaskCompleted(TaskNotification notification);
    Task TaskFailed(TaskNotification notification);
    Task TaskCancelled(TaskNotification notification);
}






