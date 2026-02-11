namespace BuildCore.Notification.Domain.DomainEvents;

/// <summary>
/// Bildirim gönderildi domain event'i
/// </summary>
public class NotificationSent
{
    public Guid NotificationId { get; }
    public string NotificationType { get; }
    public string RecipientUserId { get; }
    public DateTimeOffset SentAt { get; }

    public NotificationSent(
        Guid notificationId,
        string notificationType,
        string recipientUserId,
        DateTimeOffset sentAt)
    {
        NotificationId = notificationId;
        NotificationType = notificationType;
        RecipientUserId = recipientUserId;
        SentAt = sentAt;
    }
}
