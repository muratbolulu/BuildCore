using BuildCore.SharedKernel.Entities;

namespace BuildCore.WorkflowEngine.Domain.Entities;

/// <summary>
/// İş akışı tanımı entity'si (Aggregate Root)
/// </summary>
public class WorkflowDefinition : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string Version { get; private set; } = "1.0";
    public bool IsActive { get; private set; }
    
    // Navigation properties
    public ICollection<WorkflowStep> Steps { get; private set; } = new List<WorkflowStep>();
    public ICollection<WorkflowInstance> Instances { get; private set; } = new List<WorkflowInstance>();

    // Private constructor for EF Core
    private WorkflowDefinition() { }

    public WorkflowDefinition(
        string name,
        string? description = null,
        string version = "1.0")
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Workflow adı boş olamaz", nameof(name));

        Name = name;
        Description = description;
        Version = version;
        IsActive = true;
    }

    public void Update(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Workflow adı boş olamaz", nameof(name));

        Name = name;
        Description = description;
    }

    public void AddStep(WorkflowStep step)
    {
        if (step == null)
            throw new ArgumentNullException(nameof(step));

        Steps.Add(step);
    }

    public void RemoveStep(Guid stepId)
    {
        var step = Steps.FirstOrDefault(s => s.Id == stepId);
        if (step != null)
        {
            Steps.Remove(step);
        }
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
