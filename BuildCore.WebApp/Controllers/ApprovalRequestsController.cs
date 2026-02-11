using BuildCore.ApprovalManagement.Application.DTOs;
using BuildCore.ApprovalManagement.Application.Interfaces;
using BuildCore.WebApp.Filters;
using Microsoft.AspNetCore.Mvc;

namespace BuildCore.WebApp.Controllers;

/// <summary>
/// Onay talepleri için MVC Controller
/// </summary>
[RequireLogin]
public class ApprovalRequestsController : Controller
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

    public async Task<IActionResult> Index(string? userId, CancellationToken cancellationToken)
    {
        IEnumerable<ApprovalRequestDto> requests;

        if (!string.IsNullOrEmpty(userId))
        {
            requests = await _approvalService.GetApprovalRequestsByRequesterAsync(userId, cancellationToken);
        }
        else
        {
            // Tüm talepleri getir (basit bir yaklaşım)
            requests = new List<ApprovalRequestDto>();
        }

        return View(requests);
    }

    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var request = await _approvalService.GetApprovalRequestByIdAsync(id, cancellationToken);
        if (request == null)
        {
            return NotFound();
        }
        return View(request);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateApprovalRequestDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateApprovalRequestDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        try
        {
            await _approvalService.CreateApprovalRequestAsync(dto, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(dto);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(dto);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Hata: {ex.Message}");
            return View(dto);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(
        Guid id,
        string approverUserId,
        string? notes = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _approvalService.ApproveRequestAsync(id, approverUserId, notes, cancellationToken);
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Onay talebi onaylanırken hata oluştu.");
            TempData["Error"] = "Onay işlemi başarısız oldu.";
            return RedirectToAction(nameof(Details), new { id });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(
        Guid id,
        string rejectorUserId,
        string? reason = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _approvalService.RejectRequestAsync(id, rejectorUserId, reason, cancellationToken);
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Onay talebi reddedilirken hata oluştu.");
            TempData["Error"] = "Red işlemi başarısız oldu.";
            return RedirectToAction(nameof(Details), new { id });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(
        Guid id,
        string? cancelledByUserId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _approvalService.CancelRequestAsync(id, cancelledByUserId, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Onay talebi iptal edilirken hata oluştu.");
            return BadRequest();
        }
    }
}
