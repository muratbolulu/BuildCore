using BuildCore.SharedKernel.Interfaces;
using BuildCore.WorkflowEngine.Domain.Entities;
using BuildCore.WorkflowEngine.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuildCore.WorkflowEngine.Infrastructure.Persistence.Repositories;

/// <summary>
/// İş akışı tanımı repository implementasyonu
/// </summary>
public class WorkflowDefinitionRepository : Repository<WorkflowDefinition>, IWorkflowDefinitionRepository
{
    public WorkflowDefinitionRepository(WorkflowEngineDbContext context) : base(context)
    {
    }

    public async Task<WorkflowDefinition?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(wd => wd.Steps)
                .ThenInclude(s => s.OutgoingTransitions)
            .FirstOrDefaultAsync(wd => wd.Name == name, cancellationToken);
    }

    public async Task<IEnumerable<WorkflowDefinition>> GetActiveWorkflowsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(wd => wd.Steps)
            .Where(wd => wd.IsActive)
            .ToListAsync(cancellationToken);
    }

    public override async Task<WorkflowDefinition?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(wd => wd.Steps)
                .ThenInclude(s => s.OutgoingTransitions)
                    .ThenInclude(t => t.ToStep)
            .Include(wd => wd.Steps)
                .ThenInclude(s => s.IncomingTransitions)
            .FirstOrDefaultAsync(wd => wd.Id == id, cancellationToken);
    }

    public override async Task<IEnumerable<WorkflowDefinition>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(wd => wd.Steps)
            .ToListAsync(cancellationToken);
    }
}
