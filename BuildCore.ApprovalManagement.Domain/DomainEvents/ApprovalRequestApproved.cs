namespace BuildCore.ApprovalManagement.Domain.DomainEvents;

/// <summary>
/// Onay talebi onaylandı domain event'i
/// </summary>
public class ApprovalRequestApproved
{
    public Guid ApprovalRequestId { get; }
    public Guid ApprovalRuleId { get; }
    public string ApproverUserId { get; }
    public DateTimeOffset ApprovedAt { get; }

    public ApprovalRequestApproved(
        Guid approvalRequestId,
        Guid approvalRuleId,
        string approverUserId,
        DateTimeOffset approvedAt)
    {
        ApprovalRequestId = approvalRequestId;
        ApprovalRuleId = approvalRuleId;
        ApproverUserId = approverUserId;
        ApprovedAt = approvedAt;
    }
}
