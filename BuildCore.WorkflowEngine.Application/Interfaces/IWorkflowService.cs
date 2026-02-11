using BuildCore.WorkflowEngine.Application.DTOs;

namespace BuildCore.WorkflowEngine.Application.Interfaces;

/// <summary>
/// İş akışı servisi interface'i
/// </summary>
public interface IWorkflowService
{
    // Workflow Definition Operations
    Task<WorkflowDefinitionDto> CreateWorkflowDefinitionAsync(
        CreateWorkflowDefinitionDto dto,
        CancellationToken cancellationToken = default);

    Task<WorkflowDefinitionDto?> GetWorkflowDefinitionByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<WorkflowDefinitionDto>> GetAllWorkflowDefinitionsAsync(
        CancellationToken cancellationToken = default);

    Task<WorkflowDefinitionDto> UpdateWorkflowDefinitionAsync(
        Guid id,
        CreateWorkflowDefinitionDto dto,
        CancellationToken cancellationToken = default);

    Task DeleteWorkflowDefinitionAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    // Workflow Instance Operations
    Task<WorkflowInstanceDto> StartWorkflowInstanceAsync(
        StartWorkflowInstanceDto dto,
        CancellationToken cancellationToken = default);

    Task<WorkflowInstanceDto?> GetWorkflowInstanceByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<WorkflowInstanceDto>> GetWorkflowInstancesByDefinitionIdAsync(
        Guid workflowDefinitionId,
        CancellationToken cancellationToken = default);

    Task CompleteStepAsync(
        Guid workflowInstanceId,
        Guid stepId,
        string? userId = null,
        CancellationToken cancellationToken = default);

    Task CancelWorkflowInstanceAsync(
        Guid workflowInstanceId,
        string? userId = null,
        CancellationToken cancellationToken = default);
}
