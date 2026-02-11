using BuildCore.Notification.Domain.Entities;
using BuildCore.SharedKernel.Interfaces;
using NotificationEntity = BuildCore.Notification.Domain.Entities.Notification;

namespace BuildCore.Notification.Domain.Interfaces;

/// <summary>
/// Bildirim repository interface'i
/// </summary>
public interface INotificationRepository : IRepository<NotificationEntity>
{
    Task<IEnumerable<NotificationEntity>> GetByRecipientUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<NotificationEntity>> GetByStatusAsync(
        string status,
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<NotificationEntity>> GetPendingNotificationsAsync(
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<NotificationEntity>> GetFailedNotificationsAsync(
        CancellationToken cancellationToken = default);
}
