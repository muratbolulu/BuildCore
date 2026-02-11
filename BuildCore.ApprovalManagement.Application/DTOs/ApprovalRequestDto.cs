namespace BuildCore.ApprovalManagement.Application.DTOs;

/// <summary>
/// Onay talebi DTO'su
/// </summary>
public class ApprovalRequestDto
{
    public Guid Id { get; set; }
    public Guid ApprovalRuleId { get; set; }
    public string ApprovalRuleName { get; set; } = string.Empty;
    public string RequestType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string RequesterUserId { get; set; } = string.Empty;
    public string? RequestData { get; set; }
    public DateTimeOffset RequestedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    public List<ApprovalDecisionDto> Decisions { get; set; } = new();
}
