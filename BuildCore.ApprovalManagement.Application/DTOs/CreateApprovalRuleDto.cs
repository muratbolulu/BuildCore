using System.ComponentModel.DataAnnotations;

namespace BuildCore.ApprovalManagement.Application.DTOs;

/// <summary>
/// Onay kuralı oluşturma DTO'su
/// </summary>
public class CreateApprovalRuleDto
{
    [Required(ErrorMessage = "Onay kuralı adı zorunludur.")]
    [StringLength(200, ErrorMessage = "Ad en fazla 200 karakter olabilir.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Açıklama en fazla 1000 karakter olabilir.")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Kural tipi zorunludur.")]
    public string RuleType { get; set; } = string.Empty;

    public string? Condition { get; set; }
}
