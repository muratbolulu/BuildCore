using BuildCore.SharedKernel.Entities;

namespace BuildCore.WorkflowEngine.Domain.Entities;

/// <summary>
/// İş akışı instance geçmişi entity'si
/// </summary>
public class WorkflowInstanceHistory : BaseEntity
{
    public Guid WorkflowInstanceId { get; private set; }
    public string Action { get; private set; } = string.Empty; // Started, StepCompleted, StepRejected, Completed, Cancelled
    public string? UserId { get; private set; }
    public string? Notes { get; private set; }
    public DateTimeOffset ActionDate { get; private set; }

    // Navigation property
    public WorkflowInstance WorkflowInstance { get; private set; } = null!;

    // Private constructor for EF Core
    private WorkflowInstanceHistory() { }

    public WorkflowInstanceHistory(
        Guid workflowInstanceId,
        string action,
        string? userId = null,
        string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(action))
            throw new ArgumentException("Aksiyon boş olamaz", nameof(action));

        WorkflowInstanceId = workflowInstanceId;
        Action = action;
        UserId = userId;
        Notes = notes;
        ActionDate = DateTimeOffset.UtcNow;
    }
}
