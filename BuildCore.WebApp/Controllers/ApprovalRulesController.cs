using BuildCore.ApprovalManagement.Application.DTOs;
using BuildCore.ApprovalManagement.Application.Interfaces;
using BuildCore.WebApp.Filters;
using Microsoft.AspNetCore.Mvc;

namespace BuildCore.WebApp.Controllers;

/// <summary>
/// Onay kuralları için MVC Controller
/// </summary>
[RequireLogin]
public class ApprovalRulesController : Controller
{
    private readonly IApprovalService _approvalService;
    private readonly ILogger<ApprovalRulesController> _logger;

    public ApprovalRulesController(
        IApprovalService approvalService,
        ILogger<ApprovalRulesController> logger)
    {
        _approvalService = approvalService;
        _logger = logger;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var rules = await _approvalService.GetAllApprovalRulesAsync(cancellationToken);
        return View(rules);
    }

    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var rule = await _approvalService.GetApprovalRuleByIdAsync(id, cancellationToken);
        if (rule == null)
        {
            return NotFound();
        }
        return View(rule);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateApprovalRuleDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateApprovalRuleDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        try
        {
            await _approvalService.CreateApprovalRuleAsync(dto, cancellationToken);
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
        var rule = await _approvalService.GetApprovalRuleByIdAsync(id, cancellationToken);
        if (rule == null)
        {
            return NotFound();
        }

        var dto = new CreateApprovalRuleDto
        {
            Name = rule.Name,
            Description = rule.Description,
            RuleType = rule.RuleType,
            Condition = rule.Condition
        };

        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, CreateApprovalRuleDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        try
        {
            await _approvalService.UpdateApprovalRuleAsync(id, dto, cancellationToken);
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
        var rule = await _approvalService.GetApprovalRuleByIdAsync(id, cancellationToken);
        if (rule == null)
        {
            return NotFound();
        }
        return View(rule);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _approvalService.DeleteApprovalRuleAsync(id, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Onay kuralı silinirken hata oluştu.");
            return BadRequest();
        }
    }
}
