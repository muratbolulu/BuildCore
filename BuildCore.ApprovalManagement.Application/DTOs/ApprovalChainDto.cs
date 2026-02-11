namespace BuildCore.ApprovalManagement.Application.DTOs;

/// <summary>
/// Onay zinciri DTO'su
/// </summary>
public class ApprovalChainDto
{
    public Guid Id { get; set; }
    public Guid ApprovalRuleId { get; set; }
    public int Order { get; set; }
    public string ApproverType { get; set; } = string.Empty;
    public string? ApproverRole { get; set; }
    public string? ApproverUserId { get; set; }
    public bool IsRequired { get; set; }
    public bool CanDelegate { get; set; }
    public int? EscalationMinutes { get; set; }
}
