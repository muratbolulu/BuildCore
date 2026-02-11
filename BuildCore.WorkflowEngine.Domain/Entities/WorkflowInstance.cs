using BuildCore.SharedKernel.Entities;

namespace BuildCore.WorkflowEngine.Domain.Entities;

/// <summary>
/// İş akışı instance'ı entity'si (Aggregate Root)
/// </summary>
public class WorkflowInstance : BaseEntity
{
    public Guid WorkflowDefinitionId { get; private set; }
    public string Status { get; private set; } = "Pending"; // Pending, Running, Completed, Cancelled, Failed
    public Guid? CurrentStepId { get; private set; }
    public string? InitiatorUserId { get; private set; }
    public string? ContextData { get; private set; } // JSON data
    public DateTimeOffset? StartedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }

    // Navigation properties
    public WorkflowDefinition WorkflowDefinition { get; private set; } = null!;
    public WorkflowStep? CurrentStep { get; private set; }
    public ICollection<WorkflowInstanceHistory> History { get; private set; } = new List<WorkflowInstanceHistory>();

    // Private constructor for EF Core
    private WorkflowInstance() { }

    public WorkflowInstance(
        Guid workflowDefinitionId,
        string? initiatorUserId = null,
        string? contextData = null)
    {
        WorkflowDefinitionId = workflowDefinitionId;
        InitiatorUserId = initiatorUserId;
        ContextData = contextData;
        Status = "Pending";
    }

    public void Start(Guid? firstStepId = null)
    {
        if (Status != "Pending")
            throw new InvalidOperationException("Sadece bekleyen iş akışları başlatılabilir");

        Status = "Running";
        StartedAt = DateTimeOffset.UtcNow;
        CurrentStepId = firstStepId;
    }

    public void MoveToStep(Guid stepId)
    {
        if (Status != "Running")
            throw new InvalidOperationException("Sadece çalışan iş akışları adım değiştirebilir");

        CurrentStepId = stepId;
    }

    public void Complete()
    {
        if (Status != "Running")
            throw new InvalidOperationException("Sadece çalışan iş akışları tamamlanabilir");

        Status = "Completed";
        CompletedAt = DateTimeOffset.UtcNow;
        CurrentStepId = null;
    }

    public void Cancel()
    {
        if (Status == "Completed")
            throw new InvalidOperationException("Tamamlanmış iş akışları iptal edilemez");

        Status = "Cancelled";
        CompletedAt = DateTimeOffset.UtcNow;
    }

    public void Fail(string? reason = null)
    {
        Status = "Failed";
        CompletedAt = DateTimeOffset.UtcNow;
    }

    public void AddHistoryEntry(string action, string? userId = null, string? notes = null)
    {
        var historyEntry = new WorkflowInstanceHistory(
            Id,
            action,
            userId,
            notes);
        History.Add(historyEntry);
    }
}
