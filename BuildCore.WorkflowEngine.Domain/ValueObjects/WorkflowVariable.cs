namespace BuildCore.WorkflowEngine.Domain.ValueObjects;

/// <summary>
/// İş akışı değişkeni value object
/// </summary>
public class WorkflowVariable
{
    public string Name { get; }
    public string Value { get; }
    public string Type { get; } // String, Number, Boolean, DateTime

    public WorkflowVariable(string name, string value, string type = "String")
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Değişken adı boş olamaz", nameof(name));

        Name = name;
        Value = value;
        Type = type;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not WorkflowVariable other)
            return false;

        return Name == other.Name && Value == other.Value && Type == other.Type;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Value, Type);
    }
}
