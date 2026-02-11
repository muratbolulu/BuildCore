using BuildCore.ApprovalManagement.Application.DTOs;
using BuildCore.ApprovalManagement.Application.Interfaces;
using BuildCore.ApprovalManagement.Domain.Entities;
using BuildCore.ApprovalManagement.Domain.Interfaces;
using BuildCore.SharedKernel.Interfaces;

namespace BuildCore.ApprovalManagement.Application.UseCases;

/// <summary>
/// Onay servisi implementasyonu
/// </summary>
public class ApprovalService : IApprovalService
{
    private readonly IApprovalRuleRepository _approvalRuleRepository;
    private readonly IApprovalRequestRepository _approvalRequestRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ApprovalService(
        IApprovalRuleRepository approvalRuleRepository,
        IApprovalRequestRepository approvalRequestRepository,
        IUnitOfWork unitOfWork)
    {
        _approvalRuleRepository = approvalRuleRepository;
        _approvalRequestRepository = approvalRequestRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApprovalRuleDto> CreateApprovalRuleAsync(
        CreateApprovalRuleDto dto,
        CancellationToken cancellationToken = default)
    {
        var approvalRule = new ApprovalRule(
            dto.Name,
            dto.RuleType,
            dto.Description,
            dto.Condition);

        await _approvalRuleRepository.AddAsync(approvalRule, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToApprovalRuleDto(approvalRule);
    }

    public async Task<ApprovalRuleDto?> GetApprovalRuleByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var approvalRule = await _approvalRuleRepository.GetByIdAsync(id, cancellationToken);
        return approvalRule != null ? MapToApprovalRuleDto(approvalRule) : null;
    }

    public async Task<IEnumerable<ApprovalRuleDto>> GetAllApprovalRulesAsync(
        CancellationToken cancellationToken = default)
    {
        var approvalRules = await _approvalRuleRepository.GetAllAsync(cancellationToken);
        return approvalRules.Select(MapToApprovalRuleDto);
    }

    public async Task<ApprovalRuleDto> UpdateApprovalRuleAsync(
        Guid id,
        CreateApprovalRuleDto dto,
        CancellationToken cancellationToken = default)
    {
        var approvalRule = await _approvalRuleRepository.GetByIdAsync(id, cancellationToken);
        if (approvalRule == null)
            throw new KeyNotFoundException($"Onay kuralı bulunamadı. Id: {id}");

        approvalRule.Update(dto.Name, dto.Description, dto.Condition);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToApprovalRuleDto(approvalRule);
    }

    public async Task DeleteApprovalRuleAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var approvalRule = await _approvalRuleRepository.GetByIdAsync(id, cancellationToken);
        if (approvalRule == null)
            throw new KeyNotFoundException($"Onay kuralı bulunamadı. Id: {id}");

        await _approvalRuleRepository.DeleteAsync(id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<ApprovalRequestDto> CreateApprovalRequestAsync(
        CreateApprovalRequestDto dto,
        CancellationToken cancellationToken = default)
    {
        var approvalRule = await _approvalRuleRepository.GetByIdAsync(
            dto.ApprovalRuleId,
            cancellationToken);

        if (approvalRule == null)
            throw new KeyNotFoundException($"Onay kuralı bulunamadı. Id: {dto.ApprovalRuleId}");

        if (!approvalRule.IsActive)
            throw new InvalidOperationException("Onay kuralı aktif değil.");

        var approvalRequest = new ApprovalRequest(
            dto.ApprovalRuleId,
            dto.RequestType,
            dto.RequesterUserId,
            dto.RequestData);

        await _approvalRequestRepository.AddAsync(approvalRequest, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToApprovalRequestDto(approvalRequest, approvalRule);
    }

    public async Task<ApprovalRequestDto?> GetApprovalRequestByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var approvalRequest = await _approvalRequestRepository.GetByIdAsync(id, cancellationToken);
        if (approvalRequest == null)
            return null;

        var approvalRule = await _approvalRuleRepository.GetByIdAsync(
            approvalRequest.ApprovalRuleId,
            cancellationToken);

        return MapToApprovalRequestDto(approvalRequest, approvalRule);
    }

    public async Task<IEnumerable<ApprovalRequestDto>> GetApprovalRequestsByRequesterAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var approvalRequests = await _approvalRequestRepository.GetByRequesterUserIdAsync(
            userId,
            cancellationToken);

        var result = new List<ApprovalRequestDto>();
        foreach (var request in approvalRequests)
        {
            var rule = await _approvalRuleRepository.GetByIdAsync(
                request.ApprovalRuleId,
                cancellationToken);
            result.Add(MapToApprovalRequestDto(request, rule));
        }

        return result;
    }

    public async Task<IEnumerable<ApprovalRequestDto>> GetPendingApprovalRequestsAsync(
        string userId,
        string? role = null,
        CancellationToken cancellationToken = default)
    {
        var approvalRequests = await _approvalRequestRepository.GetPendingForApproverAsync(
            userId,
            role,
            cancellationToken);

        var result = new List<ApprovalRequestDto>();
        foreach (var request in approvalRequests)
        {
            var rule = await _approvalRuleRepository.GetByIdAsync(
                request.ApprovalRuleId,
                cancellationToken);
            result.Add(MapToApprovalRequestDto(request, rule));
        }

        return result;
    }

    public async Task ApproveRequestAsync(
        Guid approvalRequestId,
        string approverUserId,
        string? notes = null,
        CancellationToken cancellationToken = default)
    {
        var approvalRequest = await _approvalRequestRepository.GetByIdAsync(
            approvalRequestId,
            cancellationToken);

        if (approvalRequest == null)
            throw new KeyNotFoundException($"Onay talebi bulunamadı. Id: {approvalRequestId}");

        approvalRequest.Approve(approverUserId, notes);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RejectRequestAsync(
        Guid approvalRequestId,
        string rejectorUserId,
        string? reason = null,
        CancellationToken cancellationToken = default)
    {
        var approvalRequest = await _approvalRequestRepository.GetByIdAsync(
            approvalRequestId,
            cancellationToken);

        if (approvalRequest == null)
            throw new KeyNotFoundException($"Onay talebi bulunamadı. Id: {approvalRequestId}");

        approvalRequest.Reject(rejectorUserId, reason);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task CancelRequestAsync(
        Guid approvalRequestId,
        string? cancelledByUserId = null,
        CancellationToken cancellationToken = default)
    {
        var approvalRequest = await _approvalRequestRepository.GetByIdAsync(
            approvalRequestId,
            cancellationToken);

        if (approvalRequest == null)
            throw new KeyNotFoundException($"Onay talebi bulunamadı. Id: {approvalRequestId}");

        approvalRequest.Cancel(cancelledByUserId);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static ApprovalRuleDto MapToApprovalRuleDto(ApprovalRule approvalRule)
    {
        return new ApprovalRuleDto
        {
            Id = approvalRule.Id,
            Name = approvalRule.Name,
            Description = approvalRule.Description,
            RuleType = approvalRule.RuleType,
            IsActive = approvalRule.IsActive,
            Condition = approvalRule.Condition,
            CreatedAt = approvalRule.CreatedAt,
            ApprovalChains = approvalRule.ApprovalChains
                .OrderBy(ac => ac.Order)
                .Select(ac => new ApprovalChainDto
                {
                    Id = ac.Id,
                    ApprovalRuleId = ac.ApprovalRuleId,
                    Order = ac.Order,
                    ApproverType = ac.ApproverType,
                    ApproverRole = ac.ApproverRole,
                    ApproverUserId = ac.ApproverUserId,
                    IsRequired = ac.IsRequired,
                    CanDelegate = ac.CanDelegate,
                    EscalationMinutes = ac.EscalationMinutes
                })
                .ToList()
        };
    }

    private static ApprovalRequestDto MapToApprovalRequestDto(
        ApprovalRequest approvalRequest,
        ApprovalRule? approvalRule = null)
    {
        return new ApprovalRequestDto
        {
            Id = approvalRequest.Id,
            ApprovalRuleId = approvalRequest.ApprovalRuleId,
            ApprovalRuleName = approvalRule?.Name ?? string.Empty,
            RequestType = approvalRequest.RequestType,
            Status = approvalRequest.Status,
            RequesterUserId = approvalRequest.RequesterUserId,
            RequestData = approvalRequest.RequestData,
            RequestedAt = approvalRequest.RequestedAt,
            CompletedAt = approvalRequest.CompletedAt,
            Decisions = approvalRequest.Decisions
                .OrderBy(d => d.DecisionDate)
                .Select(d => new ApprovalDecisionDto
                {
                    Id = d.Id,
                    ApprovalRequestId = d.ApprovalRequestId,
                    ApproverUserId = d.ApproverUserId,
                    ApproverRole = d.ApproverRole,
                    Status = d.Status,
                    Notes = d.Notes,
                    DecisionDate = d.DecisionDate
                })
                .ToList()
        };
    }
}
