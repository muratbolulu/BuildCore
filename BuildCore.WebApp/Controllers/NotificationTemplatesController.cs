using BuildCore.Notification.Application.DTOs;
using BuildCore.Notification.Application.Interfaces;
using BuildCore.WebApp.Filters;
using Microsoft.AspNetCore.Mvc;

namespace BuildCore.WebApp.Controllers;

/// <summary>
/// Bildirim şablonları için MVC Controller
/// </summary>
[RequireLogin]
public class NotificationTemplatesController : Controller
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationTemplatesController> _logger;

    public NotificationTemplatesController(
        INotificationService notificationService,
        ILogger<NotificationTemplatesController> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var templates = await _notificationService.GetAllNotificationTemplatesAsync(cancellationToken);
        return View(templates);
    }

    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var template = await _notificationService.GetNotificationTemplateByIdAsync(id, cancellationToken);
        if (template == null)
        {
            return NotFound();
        }
        return View(template);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateNotificationTemplateDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateNotificationTemplateDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        try
        {
            await _notificationService.CreateNotificationTemplateAsync(dto, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Hata: {ex.Message}");
            return View(dto);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        var template = await _notificationService.GetNotificationTemplateByIdAsync(id, cancellationToken);
        if (template == null)
        {
            return NotFound();
        }

        var dto = new CreateNotificationTemplateDto
        {
            Name = template.Name,
            TemplateType = template.TemplateType,
            Subject = template.Subject,
            Body = template.Body
        };

        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, CreateNotificationTemplateDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        try
        {
            await _notificationService.UpdateNotificationTemplateAsync(id, dto, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Hata: {ex.Message}");
            return View(dto);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var template = await _notificationService.GetNotificationTemplateByIdAsync(id, cancellationToken);
        if (template == null)
        {
            return NotFound();
        }
        return View(template);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _notificationService.DeleteNotificationTemplateAsync(id, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Bildirim şablonu silinirken hata oluştu.");
            return BadRequest();
        }
    }
}
