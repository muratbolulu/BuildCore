using BuildCore.Notification.Domain.Entities;
using BuildCore.Notification.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using NotificationEntity = BuildCore.Notification.Domain.Entities.Notification;

namespace BuildCore.Notification.Infrastructure.Persistence.Repositories;

/// <summary>
/// Bildirim şablonu repository implementasyonu
/// </summary>
public class NotificationTemplateRepository : Repository<NotificationTemplate>, INotificationTemplateRepository
{
    public NotificationTemplateRepository(NotificationDbContext context) : base(context)
    {
    }

    public async Task<NotificationTemplate?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(nt => nt.Name == name, cancellationToken);
    }

    public async Task<IEnumerable<NotificationTemplate>> GetByTemplateTypeAsync(
        string templateType,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(nt => nt.TemplateType == templateType)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<NotificationTemplate>> GetActiveTemplatesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(nt => nt.IsActive)
            .ToListAsync(cancellationToken);
    }
}
