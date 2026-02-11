using System.ComponentModel.DataAnnotations;

namespace BuildCore.Notification.Application.DTOs;

/// <summary>
/// Bildirim gönderme DTO'su
/// </summary>
public class SendNotificationDto
{
    [Required(ErrorMessage = "Bildirim tipi zorunludur.")]
    public string NotificationType { get; set; } = string.Empty; // Email, SMS, Push, InApp

    [Required(ErrorMessage = "Alıcı kullanıcı ID'si zorunludur.")]
    public string RecipientUserId { get; set; } = string.Empty;

    public string? RecipientEmail { get; set; }
    public string? RecipientPhone { get; set; }

    [Required(ErrorMessage = "Konu zorunludur.")]
    public string Subject { get; set; } = string.Empty;

    [Required(ErrorMessage = "İçerik zorunludur.")]
    public string Body { get; set; } = string.Empty;

    public Guid? TemplateId { get; set; }
}
