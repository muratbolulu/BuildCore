using BuildCore.SharedKernel.Entities;

namespace BuildCore.ApprovalManagement.Domain.Entities;

/// <summary>
/// Onay zinciri entity'si
/// </summary>
public class ApprovalChain : BaseEntity
{
    public Guid ApprovalRuleId { get; private set; }
    public int Order { get; private set; }
    public string ApproverType { get; private set; } = string.Empty; // Role, User, Manager
    public string? ApproverRole { get; private set; }
    public string? ApproverUserId { get; private set; }
    public bool IsRequired { get; private set; }
    public bool CanDelegate { get; private set; }
    public int? EscalationMinutes { get; private set; }

    // Navigation property
    public ApprovalRule ApprovalRule { get; private set; } = null!;

    // Private constructor for EF Core
    private ApprovalChain() { }

    public ApprovalChain(
        Guid approvalRuleId,
        int order,
        string approverType,
        string? approverRole = null,
        string? approverUserId = null,
        bool isRequired = true,
        bool canDelegate = false,
        int? escalationMinutes = null)
    {
        if (string.IsNullOrWhiteSpace(approverType))
            throw new ArgumentException("Onaylayıcı tipi boş olamaz", nameof(approverType));

        ApprovalRuleId = approvalRuleId;
        Order = order;
        ApproverType = approverType;
        ApproverRole = approverRole;
        ApproverUserId = approverUserId;
        IsRequired = isRequired;
        CanDelegate = canDelegate;
        EscalationMinutes = escalationMinutes;
    }

    public void Update(
        string? approverRole = null,
        string? approverUserId = null,
        bool? isRequired = null,
        bool? canDelegate = null,
        int? escalationMinutes = null)
    {
        ApproverRole = approverRole;
        ApproverUserId = approverUserId;
        
        if (isRequired.HasValue)
            IsRequired = isRequired.Value;
            
        if (canDelegate.HasValue)
            CanDelegate = canDelegate.Value;
            
        EscalationMinutes = escalationMinutes;
    }
}
