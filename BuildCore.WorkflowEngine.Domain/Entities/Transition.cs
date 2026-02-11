using BuildCore.SharedKernel.Entities;

namespace BuildCore.WorkflowEngine.Domain.Entities;

/// <summary>
/// İş akışı geçişi (transition) entity'si
/// </summary>
public class Transition : BaseEntity
{
    public Guid FromStepId { get; private set; }
    public Guid ToStepId { get; private set; }
    public string? Condition { get; private set; } // JSON condition expression
    public string Name { get; private set; } = string.Empty;

    // Navigation properties
    public WorkflowStep FromStep { get; private set; } = null!;
    public WorkflowStep ToStep { get; private set; } = null!;

    // Private constructor for EF Core
    private Transition() { }

    public Transition(
        Guid fromStepId,
        Guid toStepId,
        string name,
        string? condition = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Geçiş adı boş olamaz", nameof(name));

        FromStepId = fromStepId;
        ToStepId = toStepId;
        Name = name;
        Condition = condition;
    }

    public void Update(string name, string? condition = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Geçiş adı boş olamaz", nameof(name));

        Name = name;
        Condition = condition;
    }
}
