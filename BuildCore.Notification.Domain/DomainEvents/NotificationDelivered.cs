namespace BuildCore.Notification.Domain.DomainEvents;

/// <summary>
/// Bildirim teslim edildi domain event'i
/// </summary>
public class NotificationDelivered
{
    public Guid NotificationId { get; }
    public string RecipientUserId { get; }
    public DateTimeOffset DeliveredAt { get; }

    public NotificationDelivered(
        Guid notificationId,
        string recipientUserId,
        DateTimeOffset deliveredAt)
    {
        NotificationId = notificationId;
        RecipientUserId = recipientUserId;
        DeliveredAt = deliveredAt;
    }
}
