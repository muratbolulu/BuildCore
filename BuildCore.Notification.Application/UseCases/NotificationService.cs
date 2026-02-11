using BuildCore.Notification.Application.DTOs;
using BuildCore.Notification.Application.Interfaces;
using BuildCore.Notification.Domain.Entities;
using BuildCore.Notification.Domain.Interfaces;
using BuildCore.SharedKernel.Interfaces;
using NotificationEntity = BuildCore.Notification.Domain.Entities.Notification;

namespace BuildCore.Notification.Application.UseCases;

/// <summary>
/// Bildirim servisi implementasyonu
/// </summary>
public class NotificationService : INotificationService
{
    private readonly INotificationTemplateRepository _templateRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public NotificationService(
        INotificationTemplateRepository templateRepository,
        INotificationRepository notificationRepository,
        IUnitOfWork unitOfWork)
    {
        _templateRepository = templateRepository;
        _notificationRepository = notificationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<NotificationTemplateDto> CreateNotificationTemplateAsync(
        CreateNotificationTemplateDto dto,
        CancellationToken cancellationToken = default)
    {
        var template = new NotificationTemplate(
            dto.Name,
            dto.TemplateType,
            dto.Subject,
            dto.Body);

        await _templateRepository.AddAsync(template, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToNotificationTemplateDto(template);
    }

    public async Task<NotificationTemplateDto?> GetNotificationTemplateByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var template = await _templateRepository.GetByIdAsync(id, cancellationToken);
        return template != null ? MapToNotificationTemplateDto(template) : null;
    }

    public async Task<IEnumerable<NotificationTemplateDto>> GetAllNotificationTemplatesAsync(
        CancellationToken cancellationToken = default)
    {
        var templates = await _templateRepository.GetAllAsync(cancellationToken);
        return templates.Select(MapToNotificationTemplateDto);
    }

    public async Task<NotificationTemplateDto> UpdateNotificationTemplateAsync(
        Guid id,
        CreateNotificationTemplateDto dto,
        CancellationToken cancellationToken = default)
    {
        var template = await _templateRepository.GetByIdAsync(id, cancellationToken);
        if (template == null)
            throw new KeyNotFoundException($"Bildirim şablonu bulunamadı. Id: {id}");

        template.Update(dto.Name, dto.Subject, dto.Body);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToNotificationTemplateDto(template);
    }

    public async Task DeleteNotificationTemplateAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var template = await _templateRepository.GetByIdAsync(id, cancellationToken);
        if (template == null)
            throw new KeyNotFoundException($"Bildirim şablonu bulunamadı. Id: {id}");

        await _templateRepository.DeleteAsync(id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<NotificationDto> SendNotificationAsync(
        SendNotificationDto dto,
        CancellationToken cancellationToken = default)
    {
        NotificationTemplate? template = null;
        NotificationEntity notification;

        if (dto.TemplateId.HasValue)
        {
            template = await _templateRepository.GetByIdAsync(dto.TemplateId.Value, cancellationToken);
        }

        // Şablon varsa, şablon bilgilerini kullan
        if (template != null && template.IsActive)
        {
            notification = new NotificationEntity(
                dto.NotificationType,
                dto.RecipientUserId,
                template.Subject,
                template.Body,
                template.Id,
                dto.RecipientEmail,
                dto.RecipientPhone);
        }
        else
        {
            notification = new NotificationEntity(
                dto.NotificationType,
                dto.RecipientUserId,
                dto.Subject,
                dto.Body,
                dto.TemplateId,
                dto.RecipientEmail,
                dto.RecipientPhone);
        }

        await _notificationRepository.AddAsync(notification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Burada gerçek bildirim gönderme işlemi yapılacak (Email, SMS, Push)
        // Şimdilik sadece kaydediyoruz
        notification.MarkAsSent();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToNotificationDto(notification);
    }

    public async Task<NotificationDto?> GetNotificationByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var notification = await _notificationRepository.GetByIdAsync(id, cancellationToken);
        return notification != null ? MapToNotificationDto(notification) : null;
    }

    public async Task<IEnumerable<NotificationDto>> GetNotificationsByRecipientAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var notifications = await _notificationRepository.GetByRecipientUserIdAsync(userId, cancellationToken);
        return notifications.Select(MapToNotificationDto);
    }

    public async Task<IEnumerable<NotificationDto>> GetPendingNotificationsAsync(
        CancellationToken cancellationToken = default)
    {
        var notifications = await _notificationRepository.GetPendingNotificationsAsync(cancellationToken);
        return notifications.Select(MapToNotificationDto);
    }

    public async Task MarkAsDeliveredAsync(
        Guid notificationId,
        CancellationToken cancellationToken = default)
    {
        var notification = await _notificationRepository.GetByIdAsync(notificationId, cancellationToken);
        if (notification == null)
            throw new KeyNotFoundException($"Bildirim bulunamadı. Id: {notificationId}");

        notification.MarkAsDelivered();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static NotificationTemplateDto MapToNotificationTemplateDto(NotificationTemplate template)
    {
        return new NotificationTemplateDto
        {
            Id = template.Id,
            Name = template.Name,
            TemplateType = template.TemplateType,
            Subject = template.Subject,
            Body = template.Body,
            IsActive = template.IsActive,
            CreatedAt = template.CreatedAt
        };
    }

    private static NotificationDto MapToNotificationDto(NotificationEntity notification)
    {
        return new NotificationDto
        {
            Id = notification.Id,
            TemplateId = notification.TemplateId,
            NotificationType = notification.NotificationType,
            RecipientUserId = notification.RecipientUserId,
            RecipientEmail = notification.RecipientEmail,
            RecipientPhone = notification.RecipientPhone,
            Subject = notification.Subject,
            Body = notification.Body,
            Status = notification.Status,
            SentAt = notification.SentAt,
            DeliveredAt = notification.DeliveredAt,
            ErrorMessage = notification.ErrorMessage,
            RetryCount = notification.RetryCount,
            CreatedAt = notification.CreatedAt
        };
    }
}
