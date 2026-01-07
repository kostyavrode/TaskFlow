namespace TaskManagement.Domain.ValueObjects;

public record Priority
{
    public static readonly Priority Low = new(1, "Low");
    public static readonly Priority Medium = new(2, "Medium");
    public static readonly Priority High = new(3, "High");
    public static readonly Priority Critical = new(4, "Critical");

    public int Level { get; }
    public string Name { get; }

    private Priority(int level, string name)
    {
        Level = level;
        Name = name;
    }

    public static Priority FromString(string priority)
    {
        return priority?.ToLowerInvariant() switch
        {
            "low" => Low,
            "medium" => Medium,
            "high" => High,
            "critical" => Critical,
            _ => Medium
        };
    }

    public static Priority FromLevel(int level)
    {
        return level switch
        {
            1 => Low,
            2 => Medium,
            3 => High,
            4 => Critical,
            _ => Medium
        };
    }

    public override string ToString() => Name;
}


