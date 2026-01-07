namespace TaskManagement.Domain.ValueObjects;

public record TaskType
{
    public static readonly TaskType Report = new("Report");
    public static readonly TaskType Email = new("Email");
    public static readonly TaskType DataProcessing = new("DataProcessing");
    public static readonly TaskType Notification = new("Notification");
    public static readonly TaskType Backup = new("Backup");

    public string Name { get; }

    private TaskType(string name)
    {
        Name = name;
    }

    public static TaskType FromString(string type)
    {
        return type?.ToLowerInvariant() switch
        {
            "report" => Report,
            "email" => Email,
            "dataprocessing" => DataProcessing,
            "notification" => Notification,
            "backup" => Backup,
            _ => Report
        };
    }

    public override string ToString() => Name;
}


