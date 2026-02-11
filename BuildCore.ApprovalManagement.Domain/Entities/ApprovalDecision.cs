using BuildCore.SharedKernel.Entities;

namespace BuildCore.ApprovalManagement.Domain.Entities;

/// <summary>
/// Onay kararı entity'si
/// </summary>
public class ApprovalDecision : BaseEntity
{
    public Guid ApprovalRequestId { get; private set; }
    public string ApproverUserId { get; private set; } = string.Empty;
    public string? ApproverRole { get; private set; }
    public string Status { get; private set; } = string.Empty; // Approved, Rejected
    public string? Notes { get; private set; }
    public DateTimeOffset DecisionDate { get; private set; }

    // Navigation property
    public ApprovalRequest ApprovalRequest { get; private set; } = null!;

    // Private constructor for EF Core
    private ApprovalDecision() { }

    public ApprovalDecision(
        Guid approvalRequestId,
        string approverUserId,
        string status,
        string? notes = null,
        string? approverRole = null)
    {
        if (string.IsNullOrWhiteSpace(approverUserId))
            throw new ArgumentException("Onaylayıcı kullanıcı ID'si boş olamaz", nameof(approverUserId));

        if (string.IsNullOrWhiteSpace(status))
            throw new ArgumentException("Durum boş olamaz", nameof(status));

        ApprovalRequestId = approvalRequestId;
        ApproverUserId = approverUserId;
        ApproverRole = approverRole;
        Status = status;
        Notes = notes;
        DecisionDate = DateTimeOffset.UtcNow;
    }
}
