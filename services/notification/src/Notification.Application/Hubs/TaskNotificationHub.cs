using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Notification.Application.Hubs;

public class TaskNotificationHub : Hub<ITaskNotificationClient>
{
    private readonly ILogger<TaskNotificationHub> _logger;

    public TaskNotificationHub(ILogger<TaskNotificationHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SubscribeToUser(string userId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
        _logger.LogInformation("Client {ConnectionId} subscribed to user {UserId}", Context.ConnectionId, userId);
    }

    public async Task UnsubscribeFromUser(string userId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user-{userId}");
        _logger.LogInformation("Client {ConnectionId} unsubscribed from user {UserId}", Context.ConnectionId, userId);
    }

    public async Task SubscribeToTask(string taskId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"task-{taskId}");
        _logger.LogInformation("Client {ConnectionId} subscribed to task {TaskId}", Context.ConnectionId, taskId);
    }

    public async Task UnsubscribeFromTask(string taskId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"task-{taskId}");
        _logger.LogInformation("Client {ConnectionId} unsubscribed from task {TaskId}", Context.ConnectionId, taskId);
    }
}






