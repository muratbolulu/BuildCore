namespace BuildCore.WorkflowEngine.Application.DTOs;

/// <summary>
/// İş akışı instance geçmişi DTO'su
/// </summary>
public class WorkflowInstanceHistoryDto
{
    public Guid Id { get; set; }
    public Guid WorkflowInstanceId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string? Notes { get; set; }
    public DateTimeOffset ActionDate { get; set; }
}
