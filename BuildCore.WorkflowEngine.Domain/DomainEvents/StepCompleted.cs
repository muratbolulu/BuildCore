namespace BuildCore.WorkflowEngine.Domain.DomainEvents;

/// <summary>
/// İş akışı adımı tamamlandı domain event'i
/// </summary>
public class StepCompleted
{
    public Guid WorkflowInstanceId { get; }
    public Guid StepId { get; }
    public string? CompletedByUserId { get; }
    public DateTimeOffset CompletedAt { get; }

    public StepCompleted(
        Guid workflowInstanceId,
        Guid stepId,
        string? completedByUserId,
        DateTimeOffset completedAt)
    {
        WorkflowInstanceId = workflowInstanceId;
        StepId = stepId;
        CompletedByUserId = completedByUserId;
        CompletedAt = completedAt;
    }
}
