namespace BuildCore.Notification.Application.DTOs;

/// <summary>
/// Bildirim şablonu DTO'su
/// </summary>
public class NotificationTemplateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string TemplateType { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
