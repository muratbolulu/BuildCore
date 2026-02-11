using BuildCore.WorkflowEngine.Application.DTOs;
using BuildCore.WorkflowEngine.Application.Interfaces;
using BuildCore.WebApp.Filters;
using Microsoft.AspNetCore.Mvc;

namespace BuildCore.WebApp.Controllers;

/// <summary>
/// İş akışı tanımları için MVC Controller
/// </summary>
[RequireLogin]
public class WorkflowDefinitionsController : Controller
{
    private readonly IWorkflowService _workflowService;
    private readonly ILogger<WorkflowDefinitionsController> _logger;

    public WorkflowDefinitionsController(
        IWorkflowService workflowService,
        ILogger<WorkflowDefinitionsController> logger)
    {
        _workflowService = workflowService;
        _logger = logger;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var workflows = await _workflowService.GetAllWorkflowDefinitionsAsync(cancellationToken);
        return View(workflows);
    }

    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var workflow = await _workflowService.GetWorkflowDefinitionByIdAsync(id, cancellationToken);
        if (workflow == null)
        {
            return NotFound();
        }
        return View(workflow);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateWorkflowDefinitionDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateWorkflowDefinitionDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        try
        {
            await _workflowService.CreateWorkflowDefinitionAsync(dto, cancellationToken);
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
        var workflow = await _workflowService.GetWorkflowDefinitionByIdAsync(id, cancellationToken);
        if (workflow == null)
        {
            return NotFound();
        }

        var dto = new CreateWorkflowDefinitionDto
        {
            Name = workflow.Name,
            Description = workflow.Description
        };

        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, CreateWorkflowDefinitionDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        try
        {
            await _workflowService.UpdateWorkflowDefinitionAsync(id, dto, cancellationToken);
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
        var workflow = await _workflowService.GetWorkflowDefinitionByIdAsync(id, cancellationToken);
        if (workflow == null)
        {
            return NotFound();
        }
        return View(workflow);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _workflowService.DeleteWorkflowDefinitionAsync(id, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "İş akışı tanımı silinirken hata oluştu.");
            return BadRequest();
        }
    }
}
