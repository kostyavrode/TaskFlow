using Microsoft.Extensions.DependencyInjection;
using TaskExecution.Application.Services;

namespace TaskExecution.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<TaskExecutionService>();
        return services;
    }
}

