using BuildCore.ApprovalManagement.Domain.Entities;
using BuildCore.SharedKernel.Interfaces;

namespace BuildCore.ApprovalManagement.Domain.Interfaces;

/// <summary>
/// Onay kuralı repository interface'i
/// </summary>
public interface IApprovalRuleRepository : IRepository<ApprovalRule>
{
    Task<ApprovalRule?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<ApprovalRule>> GetByRuleTypeAsync(string ruleType, CancellationToken cancellationToken = default);
    Task<IEnumerable<ApprovalRule>> GetActiveRulesAsync(CancellationToken cancellationToken = default);
}
