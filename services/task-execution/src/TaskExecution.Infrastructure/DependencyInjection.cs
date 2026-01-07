using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskExecution.Application.Consumers;
using TaskExecution.Application.Services;
using TaskExecution.Domain.Repositories;
using TaskExecution.Domain.Services;
using TaskExecution.Infrastructure.EventBus;
using TaskExecution.Infrastructure.Executors;
using TaskExecution.Infrastructure.Persistence;
using TaskFlow.Infrastructure.EventBus;
using TaskFlow.Infrastructure.Idempotency;

namespace TaskExecution.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<TaskExecutionDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("TaskExecutionDb")));

        services.AddScoped<IExecutionRepository, ExecutionRepository>();
        services.AddScoped<IIdempotencyService, IdempotencyService>();

        services.AddScoped<ITaskTypeHandler, ReportTaskHandler>();
        services.AddScoped<ITaskTypeHandler, EmailTaskHandler>();
        services.AddScoped<ITaskTypeHandler, DataProcessingTaskHandler>();
        services.AddScoped<ITaskExecutor, DefaultTaskExecutor>();

        var eventBusConfig = new EventBusConfiguration
        {
            Host = configuration["EventBus:Host"] ?? "localhost",
            Port = int.TryParse(configuration["EventBus:Port"], out var port) ? port : 5672,
            Username = configuration["EventBus:Username"] ?? "guest",
            Password = configuration["EventBus:Password"] ?? "guest",
            ConcurrencyLimit = int.TryParse(configuration["EventBus:ConcurrencyLimit"], out var limit) ? limit : 5
        };

        services.AddTaskFlowEventBus("task-execution", eventBusConfig, x =>
        {
            x.AddConsumer<TaskCreatedConsumer>();
            x.AddConsumer<TaskCancelledConsumer>();
        });

        services.AddScoped<IEventPublisher, MassTransitEventPublisher>();

        return services;
    }
}

