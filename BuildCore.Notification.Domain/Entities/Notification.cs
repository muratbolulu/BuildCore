using BuildCore.SharedKernel.Entities;

namespace BuildCore.Notification.Domain.Entities;

/// <summary>
/// Bildirim entity'si (Aggregate Root)
/// </summary>
public class Notification : BaseEntity
{
    public Guid? TemplateId { get; private set; }
    public string NotificationType { get; private set; } = string.Empty; // Email, SMS, Push, InApp
    public string RecipientUserId { get; private set; } = string.Empty;
    public string? RecipientEmail { get; private set; }
    public string? RecipientPhone { get; private set; }
    public string Subject { get; private set; } = string.Empty;
    public string Body { get; private set; } = string.Empty;
    public string Status { get; private set; } = "Pending"; // Pending, Sent, Delivered, Failed
    public DateTimeOffset? SentAt { get; private set; }
    public DateTimeOffset? DeliveredAt { get; private set; }
    public string? ErrorMessage { get; private set; }
    public int RetryCount { get; private set; }

    // Navigation property
    public NotificationTemplate? Template { get; private set; }

    // Private constructor for EF Core
    private Notification() { }

    public Notification(
        string notificationType,
        string recipientUserId,
        string subject,
        string body,
        Guid? templateId = null,
        string? recipientEmail = null,
        string? recipientPhone = null)
    {
        if (string.IsNullOrWhiteSpace(notificationType))
            throw new ArgumentException("Bildirim tipi boş olamaz", nameof(notificationType));

        if (string.IsNullOrWhiteSpace(recipientUserId))
            throw new ArgumentException("Alıcı kullanıcı ID'si boş olamaz", nameof(recipientUserId));

        TemplateId = templateId;
        NotificationType = notificationType;
        RecipientUserId = recipientUserId;
        RecipientEmail = recipientEmail;
        RecipientPhone = recipientPhone;
        Subject = subject;
        Body = body;
        Status = "Pending";
    }

    public void MarkAsSent()
    {
        if (Status != "Pending")
            throw new InvalidOperationException("Sadece bekleyen bildirimler gönderilmiş olarak işaretlenebilir");

        Status = "Sent";
        SentAt = DateTimeOffset.UtcNow;
    }

    public void MarkAsDelivered()
    {
        if (Status != "Sent")
            throw new InvalidOperationException("Sadece gönderilmiş bildirimler teslim edilmiş olarak işaretlenebilir");

        Status = "Delivered";
        DeliveredAt = DateTimeOffset.UtcNow;
    }

    public void MarkAsFailed(string? errorMessage = null)
    {
        Status = "Failed";
        ErrorMessage = errorMessage;
        RetryCount++;
    }

    public void Retry()
    {
        if (Status != "Failed")
            throw new InvalidOperationException("Sadece başarısız bildirimler tekrar denenebilir");

        Status = "Pending";
        ErrorMessage = null;
    }
}
