using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                var rabbitMqHost = configuration["EventBus:Host"] ?? "localhost";
                var rabbitMqPort = int.TryParse(configuration["EventBus:Port"], out var port) ? port : 5672;
                var rabbitMqUser = configuration["EventBus:Username"] ?? "guest";
                var rabbitMqPassword = configuration["EventBus:Password"] ?? "guest";

                cfg.Host(rabbitMqHost, (ushort)rabbitMqPort, "/", h =>
                {
                    h.Username(rabbitMqUser);
                    h.Password(rabbitMqPassword);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        services.AddScoped<IEventPublisher, MassTransitEventPublisher>();

        return services;
    }
}

