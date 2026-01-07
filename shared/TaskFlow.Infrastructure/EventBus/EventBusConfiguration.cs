namespace TaskFlow.Infrastructure.EventBus;

public class EventBusConfiguration
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string Username { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string VirtualHost { get; set; } = "/";
    
    public int RetryCount { get; set; } = 3;
    public int RetryIntervalSeconds { get; set; } = 5;
    public int ConcurrencyLimit { get; set; } = 10;
}

