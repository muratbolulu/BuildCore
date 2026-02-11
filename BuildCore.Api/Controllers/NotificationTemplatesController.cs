using BuildCore.Notification.Application.DTOs;
using BuildCore.Notification.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuildCore.Api.Controllers;

/// <summary>
/// Bildirim şablonları için API Controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class NotificationTemplatesController : ControllerBase
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

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<NotificationTemplateDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<NotificationTemplateDto>>> GetAll(CancellationToken cancellationToken)
    {
        var templates = await _notificationService.GetAllNotificationTemplatesAsync(cancellationToken);
        return Ok(templates);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(NotificationTemplateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<NotificationTemplateDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var template = await _notificationService.GetNotificationTemplateByIdAsync(id, cancellationToken);
        if (template == null)
            return NotFound();

        return Ok(template);
    }

    [HttpPost]
    [ProducesResponseType(typeof(NotificationTemplateDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<NotificationTemplateDto>> Create(
        [FromBody] CreateNotificationTemplateDto dto,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var template = await _notificationService.CreateNotificationTemplateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = template.Id }, template);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Bildirim şablonu oluşturulurken hata oluştu.");
            return BadRequest(new { message = "Bildirim şablonu oluşturulamadı." });
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(NotificationTemplateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<NotificationTemplateDto>> Update(
        Guid id,
        [FromBody] CreateNotificationTemplateDto dto,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var template = await _notificationService.UpdateNotificationTemplateAsync(id, dto, cancellationToken);
            return Ok(template);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Bildirim şablonu güncellenirken hata oluştu.");
            return BadRequest(new { message = "Bildirim şablonu güncellenemedi." });
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _notificationService.DeleteNotificationTemplateAsync(id, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Bildirim şablonu silinirken hata oluştu.");
            return BadRequest(new { message = "Bildirim şablonu silinemedi." });
        }
    }
}
