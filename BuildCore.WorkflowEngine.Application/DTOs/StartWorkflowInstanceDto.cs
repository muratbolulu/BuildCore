using System.ComponentModel.DataAnnotations;

namespace BuildCore.WorkflowEngine.Application.DTOs;

/// <summary>
/// İş akışı instance başlatma DTO'su
/// </summary>
public class StartWorkflowInstanceDto
{
    [Required(ErrorMessage = "İş akışı tanımı ID'si zorunludur.")]
    public Guid WorkflowDefinitionId { get; set; }

    public string? InitiatorUserId { get; set; }
    public string? ContextData { get; set; } // JSON data
}
