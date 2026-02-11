using System.ComponentModel.DataAnnotations;

namespace BuildCore.WorkflowEngine.Application.DTOs;

/// <summary>
/// İş akışı tanımı oluşturma DTO'su
/// </summary>
public class CreateWorkflowDefinitionDto
{
    [Required(ErrorMessage = "İş akışı adı zorunludur.")]
    [StringLength(200, ErrorMessage = "İş akışı adı en fazla 200 karakter olabilir.")]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Açıklama en fazla 1000 karakter olabilir.")]
    public string? Description { get; set; }

    public string Version { get; set; } = "1.0";
}
