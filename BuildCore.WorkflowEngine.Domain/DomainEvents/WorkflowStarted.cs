namespace BuildCore.WorkflowEngine.Domain.DomainEvents;

/// <summary>
/// İş akışı başlatıldı domain event'i
/// </summary>
public class WorkflowStarted
{
    public Guid WorkflowInstanceId { get; }
    public Guid WorkflowDefinitionId { get; }
    public string? InitiatorUserId { get; }
    public DateTimeOffset StartedAt { get; }

    public WorkflowStarted(
        Guid workflowInstanceId,
        Guid workflowDefinitionId,
        string? initiatorUserId,
        DateTimeOffset startedAt)
    {
        WorkflowInstanceId = workflowInstanceId;
        WorkflowDefinitionId = workflowDefinitionId;
        InitiatorUserId = initiatorUserId;
        StartedAt = startedAt;
    }
}
