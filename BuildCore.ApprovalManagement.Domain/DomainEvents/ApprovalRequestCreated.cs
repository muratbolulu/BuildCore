namespace BuildCore.ApprovalManagement.Domain.DomainEvents;

/// <summary>
/// Onay talebi oluşturuldu domain event'i
/// </summary>
public class ApprovalRequestCreated
{
    public Guid ApprovalRequestId { get; }
    public Guid ApprovalRuleId { get; }
    public string RequestType { get; }
    public string RequesterUserId { get; }
    public DateTimeOffset CreatedAt { get; }

    public ApprovalRequestCreated(
        Guid approvalRequestId,
        Guid approvalRuleId,
        string requestType,
        string requesterUserId,
        DateTimeOffset createdAt)
    {
        ApprovalRequestId = approvalRequestId;
        ApprovalRuleId = approvalRuleId;
        RequestType = requestType;
        RequesterUserId = requesterUserId;
        CreatedAt = createdAt;
    }
}
