namespace BuildCore.WorkflowEngine.Domain.DomainEvents;

/// <summary>
/// İş akışı iptal edildi domain event'i
/// </summary>
public class WorkflowCancelled
{
    public Guid WorkflowInstanceId { get; }
    public Guid WorkflowDefinitionId { get; }
    public string? CancelledByUserId { get; }
    public DateTimeOffset CancelledAt { get; }

    public WorkflowCancelled(
        Guid workflowInstanceId,
        Guid workflowDefinitionId,
        string? cancelledByUserId,
        DateTimeOffset cancelledAt)
    {
        WorkflowInstanceId = workflowInstanceId;
        WorkflowDefinitionId = workflowDefinitionId;
        CancelledByUserId = cancelledByUserId;
        CancelledAt = cancelledAt;
    }
}
