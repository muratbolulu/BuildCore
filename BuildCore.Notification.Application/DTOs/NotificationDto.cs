namespace BuildCore.Notification.Application.DTOs;

/// <summary>
/// Bildirim DTO'su
/// </summary>
public class NotificationDto
{
    public Guid Id { get; set; }
    public Guid? TemplateId { get; set; }
    public string NotificationType { get; set; } = string.Empty;
    public string RecipientUserId { get; set; } = string.Empty;
    public string? RecipientEmail { get; set; }
    public string? RecipientPhone { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset? SentAt { get; set; }
    public DateTimeOffset? DeliveredAt { get; set; }
    public string? ErrorMessage { get; set; }
    public int RetryCount { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
