using BuildCore.SharedKernel.Entities;

namespace BuildCore.WorkflowEngine.Domain.Entities;

/// <summary>
/// İş akışı adımı entity'si
/// </summary>
public class WorkflowStep : BaseEntity
{
    public Guid WorkflowDefinitionId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string StepType { get; private set; } = string.Empty; // Start, Task, Approval, End
    public int Order { get; private set; }
    public string? AssigneeRole { get; private set; }
    public string? AssigneeUserId { get; private set; }
    public bool IsRequired { get; private set; }
    public int? TimeoutMinutes { get; private set; }

    // Navigation properties
    public WorkflowDefinition WorkflowDefinition { get; private set; } = null!;
    public ICollection<Transition> OutgoingTransitions { get; private set; } = new List<Transition>();
    public ICollection<Transition> IncomingTransitions { get; private set; } = new List<Transition>();

    // Private constructor for EF Core
    private WorkflowStep() { }

    public WorkflowStep(
        Guid workflowDefinitionId,
        string name,
        string stepType,
        int order,
        string? assigneeRole = null,
        string? assigneeUserId = null,
        bool isRequired = true,
        int? timeoutMinutes = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Adım adı boş olamaz", nameof(name));

        if (string.IsNullOrWhiteSpace(stepType))
            throw new ArgumentException("Adım tipi boş olamaz", nameof(stepType));

        WorkflowDefinitionId = workflowDefinitionId;
        Name = name;
        StepType = stepType;
        Order = order;
        AssigneeRole = assigneeRole;
        AssigneeUserId = assigneeUserId;
        IsRequired = isRequired;
        TimeoutMinutes = timeoutMinutes;
    }

    public void Update(
        string name,
        string? assigneeRole = null,
        string? assigneeUserId = null,
        bool? isRequired = null,
        int? timeoutMinutes = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Adım adı boş olamaz", nameof(name));

        Name = name;
        AssigneeRole = assigneeRole;
        AssigneeUserId = assigneeUserId;
        
        if (isRequired.HasValue)
            IsRequired = isRequired.Value;
            
        TimeoutMinutes = timeoutMinutes;
    }
}
