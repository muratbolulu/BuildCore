using BuildCore.SharedKernel.Interfaces;
using BuildCore.WorkflowEngine.Domain.Entities;

namespace BuildCore.WorkflowEngine.Domain.Interfaces;

/// <summary>
/// İş akışı tanımı repository interface'i
/// </summary>
public interface IWorkflowDefinitionRepository : IRepository<WorkflowDefinition>
{
    Task<WorkflowDefinition?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<WorkflowDefinition>> GetActiveWorkflowsAsync(CancellationToken cancellationToken = default);
}
