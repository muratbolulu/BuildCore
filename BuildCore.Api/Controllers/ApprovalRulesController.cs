using BuildCore.ApprovalManagement.Application.DTOs;
using BuildCore.ApprovalManagement.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuildCore.Api.Controllers;

/// <summary>
/// Onay kuralları için API Controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class ApprovalRulesController : ControllerBase
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

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ApprovalRuleDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ApprovalRuleDto>>> GetAll(CancellationToken cancellationToken)
    {
        var rules = await _approvalService.GetAllApprovalRulesAsync(cancellationToken);
        return Ok(rules);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApprovalRuleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApprovalRuleDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var rule = await _approvalService.GetApprovalRuleByIdAsync(id, cancellationToken);
        if (rule == null)
            return NotFound();

        return Ok(rule);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApprovalRuleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApprovalRuleDto>> Create(
        [FromBody] CreateApprovalRuleDto dto,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var rule = await _approvalService.CreateApprovalRuleAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = rule.Id }, rule);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Onay kuralı oluşturulurken hata oluştu.");
            return BadRequest(new { message = "Onay kuralı oluşturulamadı." });
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApprovalRuleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApprovalRuleDto>> Update(
        Guid id,
        [FromBody] CreateApprovalRuleDto dto,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var rule = await _approvalService.UpdateApprovalRuleAsync(id, dto, cancellationToken);
            return Ok(rule);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Onay kuralı güncellenirken hata oluştu.");
            return BadRequest(new { message = "Onay kuralı güncellenemedi." });
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _approvalService.DeleteApprovalRuleAsync(id, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Onay kuralı silinirken hata oluştu.");
            return BadRequest(new { message = "Onay kuralı silinemedi." });
        }
    }
}
