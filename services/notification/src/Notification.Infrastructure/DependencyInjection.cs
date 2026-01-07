using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notification.Application.Consumers;
using TaskFlow.Infrastructure.EventBus;

namespace Notification.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var eventBusConfig = new EventBusConfiguration
        {
            Host = configuration["EventBus:Host"] ?? "localhost",
            Port = int.TryParse(configuration["EventBus:Port"], out var port) ? port : 5672,
            Username = configuration["EventBus:Username"] ?? "guest",
            Password = configuration["EventBus:Password"] ?? "guest"
        };

        services.AddTaskFlowEventBus("notification", eventBusConfig, x =>
        {
            x.AddConsumer<TaskCreatedConsumer>();
            x.AddConsumer<TaskStartedConsumer>();
            x.AddConsumer<TaskProgressConsumer>();
            x.AddConsumer<TaskCompletedConsumer>();
            x.AddConsumer<TaskFailedConsumer>();
            x.AddConsumer<TaskCancelledConsumer>();
        });

        return services;
    }
}





