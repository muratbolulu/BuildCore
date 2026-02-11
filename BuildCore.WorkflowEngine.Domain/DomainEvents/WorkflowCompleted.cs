namespace BuildCore.WorkflowEngine.Domain.DomainEvents;

/// <summary>
/// İş akışı tamamlandı domain event'i
/// </summary>
public class WorkflowCompleted
{
    public Guid WorkflowInstanceId { get; }
    public Guid WorkflowDefinitionId { get; }
    public DateTimeOffset CompletedAt { get; }

    public WorkflowCompleted(
        Guid workflowInstanceId,
        Guid workflowDefinitionId,
        DateTimeOffset completedAt)
    {
        WorkflowInstanceId = workflowInstanceId;
        WorkflowDefinitionId = workflowDefinitionId;
        CompletedAt = completedAt;
    }
}
