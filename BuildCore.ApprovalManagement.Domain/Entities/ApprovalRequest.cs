using BuildCore.SharedKernel.Entities;

namespace BuildCore.ApprovalManagement.Domain.Entities;

/// <summary>
/// Onay talebi entity'si (Aggregate Root)
/// </summary>
public class ApprovalRequest : BaseEntity
{
    public Guid ApprovalRuleId { get; private set; }
    public string RequestType { get; private set; } = string.Empty;
    public string Status { get; private set; } = "Pending"; // Pending, Approved, Rejected, Cancelled, Escalated
    public string RequesterUserId { get; private set; } = string.Empty;
    public string? RequestData { get; private set; } // JSON data
    public DateTimeOffset RequestedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }

    // Navigation properties
    public ApprovalRule ApprovalRule { get; private set; } = null!;
    public ICollection<ApprovalDecision> Decisions { get; private set; } = new List<ApprovalDecision>();

    // Private constructor for EF Core
    private ApprovalRequest() { }

    public ApprovalRequest(
        Guid approvalRuleId,
        string requestType,
        string requesterUserId,
        string? requestData = null)
    {
        if (string.IsNullOrWhiteSpace(requestType))
            throw new ArgumentException("Talep tipi boş olamaz", nameof(requestType));

        if (string.IsNullOrWhiteSpace(requesterUserId))
            throw new ArgumentException("Talep eden kullanıcı ID'si boş olamaz", nameof(requesterUserId));

        ApprovalRuleId = approvalRuleId;
        RequestType = requestType;
        RequesterUserId = requesterUserId;
        RequestData = requestData;
        Status = "Pending";
        RequestedAt = DateTimeOffset.UtcNow;
    }

    public void Approve(string approverUserId, string? notes = null)
    {
        if (Status != "Pending")
            throw new InvalidOperationException("Sadece bekleyen talepler onaylanabilir");

        var decision = new ApprovalDecision(
            Id,
            approverUserId,
            "Approved",
            notes);
        Decisions.Add(decision);

        // Tüm gerekli onaylar tamamlandı mı kontrol et
        if (AllRequiredApprovalsCompleted())
        {
            Status = "Approved";
            CompletedAt = DateTimeOffset.UtcNow;
        }
    }

    public void Reject(string approverUserId, string? reason = null)
    {
        if (Status != "Pending")
            throw new InvalidOperationException("Sadece bekleyen talepler reddedilebilir");

        var decision = new ApprovalDecision(
            Id,
            approverUserId,
            "Rejected",
            reason);
        Decisions.Add(decision);

        Status = "Rejected";
        CompletedAt = DateTimeOffset.UtcNow;
    }

    public void Cancel(string? cancelledByUserId = null)
    {
        if (Status == "Approved" || Status == "Rejected")
            throw new InvalidOperationException("Onaylanmış veya reddedilmiş talepler iptal edilemez");

        Status = "Cancelled";
        CompletedAt = DateTimeOffset.UtcNow;
    }

    public void Escalate()
    {
        if (Status != "Pending")
            throw new InvalidOperationException("Sadece bekleyen talepler escalate edilebilir");

        Status = "Escalated";
    }

    private bool AllRequiredApprovalsCompleted()
    {
        var requiredChains = ApprovalRule.ApprovalChains
            .Where(ac => ac.IsRequired)
            .OrderBy(ac => ac.Order)
            .ToList();

        foreach (var chain in requiredChains)
        {
            var hasApproval = Decisions.Any(d => 
                d.Status == "Approved" && 
                (d.ApproverUserId == chain.ApproverUserId || 
                 (chain.ApproverRole != null && d.ApproverRole == chain.ApproverRole)));

            if (!hasApproval)
                return false;
        }

        return true;
    }
}
