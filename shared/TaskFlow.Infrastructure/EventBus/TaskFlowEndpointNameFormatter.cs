using MassTransit;

namespace TaskFlow.Infrastructure.EventBus;

public class TaskFlowEndpointNameFormatter : IEndpointNameFormatter
{
    private readonly string _prefix;

    public TaskFlowEndpointNameFormatter(string serviceName)
    {
        _prefix = serviceName.ToLowerInvariant().Replace(" ", "-");
    }

    public string Separator => "-";

    public string TemporaryEndpoint(string tag)
    {
        return $"tmp-{_prefix}-{tag}";
    }

    public string Consumer<T>() where T : class, IConsumer
    {
        return SanitizeName($"{_prefix}-{typeof(T).Name}");
    }

    public string Message<T>() where T : class
    {
        return SanitizeName(typeof(T).Name);
    }

    public string Saga<T>() where T : class, ISaga
    {
        return SanitizeName($"{_prefix}-{typeof(T).Name}");
    }

    public string ExecuteActivity<T, TArguments>() 
        where T : class, IExecuteActivity<TArguments> 
        where TArguments : class
    {
        return SanitizeName($"{_prefix}-execute-{typeof(T).Name}");
    }

    public string CompensateActivity<T, TLog>() 
        where T : class, ICompensateActivity<TLog> 
        where TLog : class
    {
        return SanitizeName($"{_prefix}-compensate-{typeof(T).Name}");
    }

    public string SanitizeName(string name)
    {
        return name
            .Replace("Consumer", "")
            .Replace("Event", "")
            .ToLowerInvariant()
            .Replace("_", "-");
    }
}

