namespace BuildCore.ApprovalManagement.Domain.DomainEvents;

/// <summary>
/// Onay talebi reddedildi domain event'i
/// </summary>
public class ApprovalRequestRejected
{
    public Guid ApprovalRequestId { get; }
    public Guid ApprovalRuleId { get; }
    public string RejectorUserId { get; }
    public string? Reason { get; }
    public DateTimeOffset RejectedAt { get; }

    public ApprovalRequestRejected(
        Guid approvalRequestId,
        Guid approvalRuleId,
        string rejectorUserId,
        string? reason,
        DateTimeOffset rejectedAt)
    {
        ApprovalRequestId = approvalRequestId;
        ApprovalRuleId = approvalRuleId;
        RejectorUserId = rejectorUserId;
        Reason = reason;
        RejectedAt = rejectedAt;
    }
}
