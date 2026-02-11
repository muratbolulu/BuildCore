using BuildCore.ApprovalManagement.Domain.Entities;
using BuildCore.SharedKernel.Interfaces;

namespace BuildCore.ApprovalManagement.Domain.Interfaces;

/// <summary>
/// Onay talebi repository interface'i
/// </summary>
public interface IApprovalRequestRepository : IRepository<ApprovalRequest>
{
    Task<IEnumerable<ApprovalRequest>> GetByRequesterUserIdAsync(
        string userId,
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<ApprovalRequest>> GetByApprovalRuleIdAsync(
        Guid approvalRuleId,
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<ApprovalRequest>> GetByStatusAsync(
        string status,
        CancellationToken cancellationToken = default);
    
    Task<IEnumerable<ApprovalRequest>> GetPendingForApproverAsync(
        string userId,
        string? role = null,
        CancellationToken cancellationToken = default);
}
