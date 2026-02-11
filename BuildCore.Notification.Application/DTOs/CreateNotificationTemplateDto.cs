using System.ComponentModel.DataAnnotations;

namespace BuildCore.Notification.Application.DTOs;

/// <summary>
/// Bildirim şablonu oluşturma DTO'su
/// </summary>
public class CreateNotificationTemplateDto
{
    [Required(ErrorMessage = "Şablon adı zorunludur.")]
    [StringLength(200, ErrorMessage = "Ad en fazla 200 karakter olabilir.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şablon tipi zorunludur.")]
    public string TemplateType { get; set; } = string.Empty;

    [Required(ErrorMessage = "Konu zorunludur.")]
    [StringLength(500, ErrorMessage = "Konu en fazla 500 karakter olabilir.")]
    public string Subject { get; set; } = string.Empty;

    [Required(ErrorMessage = "İçerik zorunludur.")]
    public string Body { get; set; } = string.Empty;
}
