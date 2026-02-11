using BuildCore.SharedKernel.Interfaces;
using BuildCore.WorkflowEngine.Domain.Entities;

namespace BuildCore.WorkflowEngine.Domain.Interfaces;

/// <summary>
/// İş akışı instance repository interface'i
/// </summary>
public interface IWorkflowInstanceRepository : IRepository<WorkflowInstance>
{
    Task<IEnumerable<WorkflowInstance>> GetByWorkflowDefinitionIdAsync(
        Guid workflowDefinitionId,
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<WorkflowInstance>> GetByInitiatorUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<WorkflowInstance>> GetByStatusAsync(
        string status,
        CancellationToken cancellationToken = default);
}
