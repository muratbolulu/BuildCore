using BuildCore.WorkflowEngine.Application.DTOs;
using BuildCore.WorkflowEngine.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuildCore.Api.Controllers;

/// <summary>
/// İş akışı instance'ları için API Controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class WorkflowInstancesController : ControllerBase
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

    /// <summary>
    /// Yeni iş akışı instance'ı başlatır
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(WorkflowInstanceDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<WorkflowInstanceDto>> Start(
        [FromBody] StartWorkflowInstanceDto dto,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var instance = await _workflowService.StartWorkflowInstanceAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = instance.Id }, instance);
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
            _logger.LogError(ex, "İş akışı instance başlatılırken hata oluştu.");
            return BadRequest(new { message = "İş akışı instance başlatılamadı." });
        }
    }

    /// <summary>
    /// ID'ye göre iş akışı instance getirir
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(WorkflowInstanceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkflowInstanceDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var instance = await _workflowService.GetWorkflowInstanceByIdAsync(id, cancellationToken);
        if (instance == null)
            return NotFound();

        return Ok(instance);
    }

    /// <summary>
    /// İş akışı tanımına göre instance'ları getirir
    /// </summary>
    [HttpGet("definition/{workflowDefinitionId}")]
    [ProducesResponseType(typeof(IEnumerable<WorkflowInstanceDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<WorkflowInstanceDto>>> GetByDefinitionId(
        Guid workflowDefinitionId,
        CancellationToken cancellationToken)
    {
        var instances = await _workflowService.GetWorkflowInstancesByDefinitionIdAsync(
            workflowDefinitionId,
            cancellationToken);
        return Ok(instances);
    }

    /// <summary>
    /// İş akışı adımını tamamlar
    /// </summary>
    [HttpPost("{workflowInstanceId}/steps/{stepId}/complete")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CompleteStep(
        Guid workflowInstanceId,
        Guid stepId,
        [FromQuery] string? userId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _workflowService.CompleteStepAsync(workflowInstanceId, stepId, userId, cancellationToken);
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
            _logger.LogError(ex, "Adım tamamlanırken hata oluştu.");
            return BadRequest(new { message = "Adım tamamlanamadı." });
        }
    }

    /// <summary>
    /// İş akışı instance'ını iptal eder
    /// </summary>
    [HttpPost("{id}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(
        Guid id,
        [FromQuery] string? userId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _workflowService.CancelWorkflowInstanceAsync(id, userId, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "İş akışı iptal edilirken hata oluştu.");
            return BadRequest(new { message = "İş akışı iptal edilemedi." });
        }
    }
}
