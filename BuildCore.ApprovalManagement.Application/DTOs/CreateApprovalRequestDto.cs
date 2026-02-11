using System.ComponentModel.DataAnnotations;

namespace BuildCore.ApprovalManagement.Application.DTOs;

/// <summary>
/// Onay talebi oluşturma DTO'su
/// </summary>
public class CreateApprovalRequestDto
{
    [Required(ErrorMessage = "Onay kuralı ID'si zorunludur.")]
    public Guid ApprovalRuleId { get; set; }

    [Required(ErrorMessage = "Talep tipi zorunludur.")]
    public string RequestType { get; set; } = string.Empty;

    [Required(ErrorMessage = "Talep eden kullanıcı ID'si zorunludur.")]
    public string RequesterUserId { get; set; } = string.Empty;

    public string? RequestData { get; set; } // JSON data
}
