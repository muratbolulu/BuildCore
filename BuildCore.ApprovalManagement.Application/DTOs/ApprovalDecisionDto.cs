namespace BuildCore.ApprovalManagement.Application.DTOs;

/// <summary>
/// Onay kararı DTO'su
/// </summary>
public class ApprovalDecisionDto
{
    public Guid Id { get; set; }
    public Guid ApprovalRequestId { get; set; }
    public string ApproverUserId { get; set; } = string.Empty;
    public string? ApproverRole { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTimeOffset DecisionDate { get; set; }
}
