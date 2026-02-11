using BuildCore.ApprovalManagement.Domain.Entities;
using BuildCore.ApprovalManagement.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuildCore.ApprovalManagement.Infrastructure.Persistence.Repositories;

/// <summary>
/// Onay talebi repository implementasyonu
/// </summary>
public class ApprovalRequestRepository : Repository<ApprovalRequest>, IApprovalRequestRepository
{
    public ApprovalRequestRepository(ApprovalManagementDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ApprovalRequest>> GetByRequesterUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(ar => ar.Decisions)
            .Include(ar => ar.ApprovalRule)
                .ThenInclude(rule => rule.ApprovalChains)
            .Where(ar => ar.RequesterUserId == userId)
            .OrderByDescending(ar => ar.RequestedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ApprovalRequest>> GetByApprovalRuleIdAsync(
        Guid approvalRuleId,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(ar => ar.Decisions)
            .Where(ar => ar.ApprovalRuleId == approvalRuleId)
            .OrderByDescending(ar => ar.RequestedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ApprovalRequest>> GetByStatusAsync(
        string status,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(ar => ar.Decisions)
            .Include(ar => ar.ApprovalRule)
            .Where(ar => ar.Status == status)
            .OrderByDescending(ar => ar.RequestedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ApprovalRequest>> GetPendingForApproverAsync(
        string userId,
        string? role = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .Include(ar => ar.Decisions)
            .Include(ar => ar.ApprovalRule)
                .ThenInclude(rule => rule.ApprovalChains)
            .Where(ar => ar.Status == "Pending");

        // Kullanıcıya veya rolüne göre filtrele
        query = query.Where(ar =>
            ar.ApprovalRule.ApprovalChains.Any(ac =>
                (ac.ApproverUserId == userId || (role != null && ac.ApproverRole == role)) &&
                !ar.Decisions.Any(d => d.ApproverUserId == userId && d.Status == "Approved")));

        return await query
            .OrderByDescending(ar => ar.RequestedAt)
            .ToListAsync(cancellationToken);
    }

    public override async Task<ApprovalRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(ar => ar.Decisions)
            .Include(ar => ar.ApprovalRule)
                .ThenInclude(rule => rule.ApprovalChains)
            .FirstOrDefaultAsync(ar => ar.Id == id, cancellationToken);
    }

    public override async Task<IEnumerable<ApprovalRequest>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(ar => ar.Decisions)
            .Include(ar => ar.ApprovalRule)
            .OrderByDescending(ar => ar.RequestedAt)
            .ToListAsync(cancellationToken);
    }
}
