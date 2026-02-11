namespace BuildCore.WorkflowEngine.Application.DTOs;

/// <summary>
/// İş akışı tanımı DTO'su
/// </summary>
public class WorkflowDefinitionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Version { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public List<WorkflowStepDto> Steps { get; set; } = new();
}
