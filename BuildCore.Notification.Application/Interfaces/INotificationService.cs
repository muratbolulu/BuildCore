using BuildCore.Notification.Application.DTOs;

namespace BuildCore.Notification.Application.Interfaces;

/// <summary>
/// Bildirim servisi interface'i
/// </summary>
public interface INotificationService
{
    // Notification Template Operations
    Task<NotificationTemplateDto> CreateNotificationTemplateAsync(
        CreateNotificationTemplateDto dto,
        CancellationToken cancellationToken = default);

    Task<NotificationTemplateDto?> GetNotificationTemplateByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<NotificationTemplateDto>> GetAllNotificationTemplatesAsync(
        CancellationToken cancellationToken = default);

    Task<NotificationTemplateDto> UpdateNotificationTemplateAsync(
        Guid id,
        CreateNotificationTemplateDto dto,
        CancellationToken cancellationToken = default);

    Task DeleteNotificationTemplateAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    // Notification Operations
    Task<NotificationDto> SendNotificationAsync(
        SendNotificationDto dto,
        CancellationToken cancellationToken = default);

    Task<NotificationDto?> GetNotificationByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<NotificationDto>> GetNotificationsByRecipientAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<NotificationDto>> GetPendingNotificationsAsync(
        CancellationToken cancellationToken = default);

    Task MarkAsDeliveredAsync(
        Guid notificationId,
        CancellationToken cancellationToken = default);
}
