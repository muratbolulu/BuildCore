using BuildCore.ApprovalManagement.Domain.Entities;
using BuildCore.ApprovalManagement.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuildCore.ApprovalManagement.Infrastructure.Persistence.Repositories;

/// <summary>
/// Onay kuralı repository implementasyonu
/// </summary>
public class ApprovalRuleRepository : Repository<ApprovalRule>, IApprovalRuleRepository
{
    public ApprovalRuleRepository(ApprovalManagementDbContext context) : base(context)
    {
    }

    public async Task<ApprovalRule?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(ar => ar.ApprovalChains)
            .FirstOrDefaultAsync(ar => ar.Name == name, cancellationToken);
    }

    public async Task<IEnumerable<ApprovalRule>> GetByRuleTypeAsync(
        string ruleType,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(ar => ar.ApprovalChains)
            .Where(ar => ar.RuleType == ruleType)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ApprovalRule>> GetActiveRulesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(ar => ar.ApprovalChains)
            .Where(ar => ar.IsActive)
            .ToListAsync(cancellationToken);
    }

    public override async Task<ApprovalRule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(ar => ar.ApprovalChains)
            .FirstOrDefaultAsync(ar => ar.Id == id, cancellationToken);
    }

    public override async Task<IEnumerable<ApprovalRule>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(ar => ar.ApprovalChains)
            .ToListAsync(cancellationToken);
    }
}
