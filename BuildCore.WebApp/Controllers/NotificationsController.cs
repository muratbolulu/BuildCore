using BuildCore.Notification.Application.DTOs;
using BuildCore.Notification.Application.Interfaces;
using BuildCore.WebApp.Filters;
using Microsoft.AspNetCore.Mvc;

namespace BuildCore.WebApp.Controllers;

/// <summary>
/// Bildirimler için MVC Controller
/// </summary>
[RequireLogin]
public class NotificationsController : Controller
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(
        INotificationService notificationService,
        ILogger<NotificationsController> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string? userId, CancellationToken cancellationToken)
    {
        IEnumerable<NotificationDto> notifications;

        if (!string.IsNullOrEmpty(userId))
        {
            notifications = await _notificationService.GetNotificationsByRecipientAsync(userId, cancellationToken);
        }
        else
        {
            notifications = new List<NotificationDto>();
        }

        return View(notifications);
    }

    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var notification = await _notificationService.GetNotificationByIdAsync(id, cancellationToken);
        if (notification == null)
        {
            return NotFound();
        }
        return View(notification);
    }

    [HttpGet]
    public IActionResult Send()
    {
        return View(new SendNotificationDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Send(SendNotificationDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        try
        {
            await _notificationService.SendNotificationAsync(dto, cancellationToken);
            TempData["Success"] = "Bildirim başarıyla gönderildi.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Hata: {ex.Message}");
            return View(dto);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAsDelivered(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            await _notificationService.MarkAsDeliveredAsync(id, cancellationToken);
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Bildirim teslim edilmiş olarak işaretlenirken hata oluştu.");
            return BadRequest();
        }
    }
}
