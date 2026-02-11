using BuildCore.ApprovalManagement.Application.DTOs;

namespace BuildCore.ApprovalManagement.Application.Interfaces;

/// <summary>
/// Onay servisi interface'i
/// </summary>
public interface IApprovalService
{
    // Approval Rule Operations
    Task<ApprovalRuleDto> CreateApprovalRuleAsync(
        CreateApprovalRuleDto dto,
        CancellationToken cancellationToken = default);

    Task<ApprovalRuleDto?> GetApprovalRuleByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<ApprovalRuleDto>> GetAllApprovalRulesAsync(
        CancellationToken cancellationToken = default);

    Task<ApprovalRuleDto> UpdateApprovalRuleAsync(
        Guid id,
        CreateApprovalRuleDto dto,
        CancellationToken cancellationToken = default);

    Task DeleteApprovalRuleAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    // Approval Request Operations
    Task<ApprovalRequestDto> CreateApprovalRequestAsync(
        CreateApprovalRequestDto dto,
        CancellationToken cancellationToken = default);

    Task<ApprovalRequestDto?> GetApprovalRequestByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<ApprovalRequestDto>> GetApprovalRequestsByRequesterAsync(
        string userId,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<ApprovalRequestDto>> GetPendingApprovalRequestsAsync(
        string userId,
        string? role = null,
        CancellationToken cancellationToken = default);

    Task ApproveRequestAsync(
        Guid approvalRequestId,
        string approverUserId,
        string? notes = null,
        CancellationToken cancellationToken = default);

    Task RejectRequestAsync(
        Guid approvalRequestId,
        string rejectorUserId,
        string? reason = null,
        CancellationToken cancellationToken = default);

    Task CancelRequestAsync(
        Guid approvalRequestId,
        string? cancelledByUserId = null,
        CancellationToken cancellationToken = default);
}
