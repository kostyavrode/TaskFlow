using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace TaskFlow.Infrastructure.EventBus;

public static class EventBusExtensions
{
    public static IServiceCollection AddTaskFlowEventBus(
        this IServiceCollection services,
        string serviceName,
        EventBusConfiguration config,
        Action<IBusRegistrationConfigurator>? configureConsumers = null)
    {
        services.AddMassTransit(x =>
        {
            x.SetEndpointNameFormatter(new TaskFlowEndpointNameFormatter(serviceName));

            configureConsumers?.Invoke(x);

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(config.Host, (ushort)config.Port, config.VirtualHost, h =>
                {
                    h.Username(config.Username);
                    h.Password(config.Password);
                });

                cfg.UseMessageRetry(r =>
                {
                    r.Intervals(
                        TimeSpan.FromSeconds(config.RetryIntervalSeconds),
                        TimeSpan.FromSeconds(config.RetryIntervalSeconds * 2),
                        TimeSpan.FromSeconds(config.RetryIntervalSeconds * 4)
                    );
                });

                cfg.PrefetchCount = config.ConcurrencyLimit;

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}

