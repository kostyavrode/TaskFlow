using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Infrastructure.EventBus;
using TaskFlow.Infrastructure.Idempotency;
using TaskFlow.Infrastructure.Outbox;
using TaskManagement.Application.Tasks.Consumers;
using TaskManagement.Domain.Repositories;
using TaskManagement.Domain.Services;
using TaskManagement.Infrastructure.EventBus;
using TaskManagement.Infrastructure.Persistence;

namespace TaskManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<TaskManagementDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("TaskManagementDb")));

        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<IOutboxRepository, OutboxRepository>();
        services.AddScoped<IIdempotencyService, IdempotencyService>();

        var eventBusConfig = new EventBusConfiguration
        {
            Host = configuration["EventBus:Host"] ?? "localhost",
            Port = int.TryParse(configuration["EventBus:Port"], out var port) ? port : 5672,
            Username = configuration["EventBus:Username"] ?? "guest",
            Password = configuration["EventBus:Password"] ?? "guest"
        };

        services.AddTaskFlowEventBus("task-management", eventBusConfig, x =>
        {
            x.AddConsumer<TaskStartedConsumer>();
            x.AddConsumer<TaskCompletedConsumer>();
            x.AddConsumer<TaskFailedConsumer>();
        });

        services.AddScoped<IEventPublisher, MassTransitEventPublisher>();

        services.AddHostedService<OutboxProcessor>();

        return services;
    }
}
