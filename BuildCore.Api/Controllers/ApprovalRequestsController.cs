using BuildCore.ApprovalManagement.Application.DTOs;
using BuildCore.ApprovalManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuildCore.Api.Controllers;

/// <summary>
/// Onay talepleri için API Controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class ApprovalRequestsController : ControllerBase
{
    private readonly IApprovalService _approvalService;
    private readonly ILogger<ApprovalRequestsController> _logger;

    public ApprovalRequestsController(
        IApprovalService approvalService,
        ILogger<ApprovalRequestsController> logger)
    {
        _approvalService = approvalService;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApprovalRequestDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApprovalRequestDto>> Create(
        [FromBody] CreateApprovalRequestDto dto,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var request = await _approvalService.CreateApprovalRequestAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = request.Id }, request);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Onay talebi oluşturulurken hata oluştu.");
            return BadRequest(new { message = "Onay talebi oluşturulamadı." });
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApprovalRequestDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApprovalRequestDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var request = await _approvalService.GetApprovalRequestByIdAsync(id, cancellationToken);
        if (request == null)
            return NotFound();

        return Ok(request);
    }

    [HttpGet("requester/{userId}")]
    [ProducesResponseType(typeof(IEnumerable<ApprovalRequestDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ApprovalRequestDto>>> GetByRequester(
        string userId,
        CancellationToken cancellationToken)
    {
        var requests = await _approvalService.GetApprovalRequestsByRequesterAsync(userId, cancellationToken);
        return Ok(requests);
    }

    [HttpGet("pending")]
    [ProducesResponseType(typeof(IEnumerable<ApprovalRequestDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ApprovalRequestDto>>> GetPending(
        [FromQuery] string userId,
        [FromQuery] string? role = null,
        CancellationToken cancellationToken = default)
    {
        var requests = await _approvalService.GetPendingApprovalRequestsAsync(userId, role, cancellationToken);
        return Ok(requests);
    }

    [HttpPost("{id}/approve")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Approve(
        Guid id,
        [FromQuery] string approverUserId,
        [FromBody] string? notes = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _approvalService.ApproveRequestAsync(id, approverUserId, notes, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Onay talebi onaylanırken hata oluştu.");
            return BadRequest(new { message = "Onay talebi onaylanamadı." });
        }
    }

    [HttpPost("{id}/reject")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Reject(
        Guid id,
        [FromQuery] string rejectorUserId,
        [FromBody] string? reason = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _approvalService.RejectRequestAsync(id, rejectorUserId, reason, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Onay talebi reddedilirken hata oluştu.");
            return BadRequest(new { message = "Onay talebi reddedilemedi." });
        }
    }

    [HttpPost("{id}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(
        Guid id,
        [FromQuery] string? cancelledByUserId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _approvalService.CancelRequestAsync(id, cancelledByUserId, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Onay talebi iptal edilirken hata oluştu.");
            return BadRequest(new { message = "Onay talebi iptal edilemedi." });
        }
    }
}
