using BuildCore.WorkflowEngine.Application.DTOs;
using BuildCore.WorkflowEngine.Application.Interfaces;
using BuildCore.WebApp.Filters;
using Microsoft.AspNetCore.Mvc;

namespace BuildCore.WebApp.Controllers;

/// <summary>
/// İş akışı instance'ları için MVC Controller
/// </summary>
[RequireLogin]
public class WorkflowInstancesController : Controller
{
    private readonly IWorkflowService _workflowService;
    private readonly ILogger<WorkflowInstancesController> _logger;

    public WorkflowInstancesController(
        IWorkflowService workflowService,
        ILogger<WorkflowInstancesController> logger)
    {
        _workflowService = workflowService;
        _logger = logger;
    }

    public async Task<IActionResult> Index(Guid? workflowDefinitionId, CancellationToken cancellationToken)
    {
        IEnumerable<WorkflowInstanceDto> instances;

        if (workflowDefinitionId.HasValue)
        {
            instances = await _workflowService.GetWorkflowInstancesByDefinitionIdAsync(
                workflowDefinitionId.Value,
                cancellationToken);
        }
        else
        {
            // Tüm instance'ları getirmek için boş liste döndürüyoruz
            // İleride GetAllWorkflowInstancesAsync metodu eklendiğinde kullanılabilir
            instances = new List<WorkflowInstanceDto>();
        }

        return View(instances);
    }

    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        var instance = await _workflowService.GetWorkflowInstanceByIdAsync(id, cancellationToken);
        if (instance == null)
        {
            return NotFound();
        }
        return View(instance);
    }

    [HttpGet]
    public IActionResult Start()
    {
        return View(new StartWorkflowInstanceDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Start(StartWorkflowInstanceDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        try
        {
            await _workflowService.StartWorkflowInstanceAsync(dto, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException ex)
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
    public async Task<IActionResult> CompleteStep(
        Guid workflowInstanceId,
        Guid stepId,
        string? userId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _workflowService.CompleteStepAsync(workflowInstanceId, stepId, userId, cancellationToken);
            return RedirectToAction(nameof(Details), new { id = workflowInstanceId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Adım tamamlanırken hata oluştu.");
            return BadRequest();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(Guid id, string? userId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            await _workflowService.CancelWorkflowInstanceAsync(id, userId, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "İş akışı iptal edilirken hata oluştu.");
            return BadRequest();
        }
    }
}
