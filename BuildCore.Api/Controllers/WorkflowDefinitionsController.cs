using BuildCore.WorkflowEngine.Application.DTOs;
using BuildCore.WorkflowEngine.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuildCore.Api.Controllers;

/// <summary>
/// İş akışı tanımları için API Controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class WorkflowDefinitionsController : ControllerBase
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

    /// <summary>
    /// Tüm iş akışı tanımlarını getirir
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<WorkflowDefinitionDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<WorkflowDefinitionDto>>> GetAll(CancellationToken cancellationToken)
    {
        var workflows = await _workflowService.GetAllWorkflowDefinitionsAsync(cancellationToken);
        return Ok(workflows);
    }

    /// <summary>
    /// ID'ye göre iş akışı tanımı getirir
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(WorkflowDefinitionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WorkflowDefinitionDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var workflow = await _workflowService.GetWorkflowDefinitionByIdAsync(id, cancellationToken);
        if (workflow == null)
            return NotFound();

        return Ok(workflow);
    }

    /// <summary>
    /// Yeni iş akışı tanımı oluşturur
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(WorkflowDefinitionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<WorkflowDefinitionDto>> Create(
        [FromBody] CreateWorkflowDefinitionDto dto,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var workflow = await _workflowService.CreateWorkflowDefinitionAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = workflow.Id }, workflow);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "İş akışı tanımı oluşturulurken hata oluştu.");
            return BadRequest(new { message = "İş akışı tanımı oluşturulamadı." });
        }
    }

    /// <summary>
    /// İş akışı tanımını günceller
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(WorkflowDefinitionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<WorkflowDefinitionDto>> Update(
        Guid id,
        [FromBody] CreateWorkflowDefinitionDto dto,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var workflow = await _workflowService.UpdateWorkflowDefinitionAsync(id, dto, cancellationToken);
            return Ok(workflow);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "İş akışı tanımı güncellenirken hata oluştu.");
            return BadRequest(new { message = "İş akışı tanımı güncellenemedi." });
        }
    }

    /// <summary>
    /// İş akışı tanımını siler
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _workflowService.DeleteWorkflowDefinitionAsync(id, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "İş akışı tanımı silinirken hata oluştu.");
            return BadRequest(new { message = "İş akışı tanımı silinemedi." });
        }
    }
}
