using BuildCore.Notification.Application.DTOs;
using BuildCore.Notification.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuildCore.Api.Controllers;

/// <summary>
/// Bildirimler için API Controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class NotificationsController : ControllerBase
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

    [HttpPost]
    [ProducesResponseType(typeof(NotificationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<NotificationDto>> Send(
        [FromBody] SendNotificationDto dto,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var notification = await _notificationService.SendNotificationAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = notification.Id }, notification);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Bildirim gönderilirken hata oluştu.");
            return BadRequest(new { message = "Bildirim gönderilemedi." });
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(NotificationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<NotificationDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var notification = await _notificationService.GetNotificationByIdAsync(id, cancellationToken);
        if (notification == null)
            return NotFound();

        return Ok(notification);
    }

    [HttpGet("recipient/{userId}")]
    [ProducesResponseType(typeof(IEnumerable<NotificationDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<NotificationDto>>> GetByRecipient(
        string userId,
        CancellationToken cancellationToken)
    {
        var notifications = await _notificationService.GetNotificationsByRecipientAsync(userId, cancellationToken);
        return Ok(notifications);
    }

    [HttpGet("pending")]
    [ProducesResponseType(typeof(IEnumerable<NotificationDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<NotificationDto>>> GetPending(CancellationToken cancellationToken)
    {
        var notifications = await _notificationService.GetPendingNotificationsAsync(cancellationToken);
        return Ok(notifications);
    }

    [HttpPost("{id}/delivered")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsDelivered(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _notificationService.MarkAsDeliveredAsync(id, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Bildirim teslim edilmiş olarak işaretlenirken hata oluştu.");
            return BadRequest(new { message = "Bildirim teslim edilmiş olarak işaretlenemedi." });
        }
    }
}
