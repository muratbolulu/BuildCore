namespace BuildCore.WorkflowEngine.Application.DTOs;

/// <summary>
/// İş akışı adımı DTO'su
/// </summary>
public class WorkflowStepDto
{
    public Guid Id { get; set; }
    public Guid WorkflowDefinitionId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string StepType { get; set; } = string.Empty;
    public int Order { get; set; }
    public string? AssigneeRole { get; set; }
    public string? AssigneeUserId { get; set; }
    public bool IsRequired { get; set; }
    public int? TimeoutMinutes { get; set; }
}
