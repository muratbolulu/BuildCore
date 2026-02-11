namespace BuildCore.WorkflowEngine.Application.DTOs;

/// <summary>
/// İş akışı instance DTO'su
/// </summary>
public class WorkflowInstanceDto
{
    public Guid Id { get; set; }
    public Guid WorkflowDefinitionId { get; set; }
    public string WorkflowDefinitionName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public Guid? CurrentStepId { get; set; }
    public string? CurrentStepName { get; set; }
    public string? InitiatorUserId { get; set; }
    public string? ContextData { get; set; }
    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public List<WorkflowInstanceHistoryDto> History { get; set; } = new();
}
