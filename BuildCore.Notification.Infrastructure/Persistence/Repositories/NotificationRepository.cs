using BuildCore.Notification.Domain.Entities;
using BuildCore.Notification.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using NotificationEntity = BuildCore.Notification.Domain.Entities.Notification;

namespace BuildCore.Notification.Infrastructure.Persistence.Repositories;

/// <summary>
/// Bildirim repository implementasyonu
/// </summary>
public class NotificationRepository : Repository<NotificationEntity>, INotificationRepository
{
    public NotificationRepository(NotificationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<NotificationEntity>> GetByRecipientUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(n => n.RecipientUserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<NotificationEntity>> GetByStatusAsync(
        string status,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(n => n.Status == status)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<NotificationEntity>> GetPendingNotificationsAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(n => n.Status == "Pending")
            .OrderBy(n => n.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<NotificationEntity>> GetFailedNotificationsAsync(
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(n => n.Status == "Failed")
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
