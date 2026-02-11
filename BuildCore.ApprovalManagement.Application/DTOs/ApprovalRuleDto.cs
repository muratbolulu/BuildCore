namespace BuildCore.ApprovalManagement.Application.DTOs;

/// <summary>
/// Onay kuralı DTO'su
/// </summary>
public class ApprovalRuleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string RuleType { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string? Condition { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public List<ApprovalChainDto> ApprovalChains { get; set; } = new();
}
