using BuildCore.Notification.Domain.Entities;
using BuildCore.SharedKernel.Interfaces;

namespace BuildCore.Notification.Domain.Interfaces;

/// <summary>
/// Bildirim şablonu repository interface'i
/// </summary>
public interface INotificationTemplateRepository : IRepository<NotificationTemplate>
{
    Task<NotificationTemplate?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<NotificationTemplate>> GetByTemplateTypeAsync(string templateType, CancellationToken cancellationToken = default);
    Task<IEnumerable<NotificationTemplate>> GetActiveTemplatesAsync(CancellationToken cancellationToken = default);
}
