using BuildCore.SharedKernel.Interfaces;
using BuildCore.WorkflowEngine.Domain.Entities;
using BuildCore.WorkflowEngine.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuildCore.WorkflowEngine.Infrastructure.Persistence.Repositories;

/// <summary>
/// İş akışı instance repository implementasyonu
/// </summary>
public class WorkflowInstanceRepository : Repository<WorkflowInstance>, IWorkflowInstanceRepository
{
    public WorkflowInstanceRepository(WorkflowEngineDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<WorkflowInstance>> GetByWorkflowDefinitionIdAsync(
        Guid workflowDefinitionId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(wi => wi.History)
            .Where(wi => wi.WorkflowDefinitionId == workflowDefinitionId)
            .OrderByDescending(wi => wi.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WorkflowInstance>> GetByInitiatorUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(wi => wi.History)
            .Where(wi => wi.InitiatorUserId == userId)
            .OrderByDescending(wi => wi.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WorkflowInstance>> GetByStatusAsync(
        string status,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(wi => wi.History)
            .Where(wi => wi.Status == status)
            .OrderByDescending(wi => wi.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public override async Task<WorkflowInstance?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(wi => wi.History)
            .Include(wi => wi.WorkflowDefinition)
                .ThenInclude(wd => wd.Steps)
            .Include(wi => wi.CurrentStep)
            .FirstOrDefaultAsync(wi => wi.Id == id, cancellationToken);
    }

    public override async Task<IEnumerable<WorkflowInstance>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(wi => wi.History)
            .OrderByDescending(wi => wi.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
