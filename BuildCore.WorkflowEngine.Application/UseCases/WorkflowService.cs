using BuildCore.SharedKernel.Interfaces;
using BuildCore.WorkflowEngine.Application.DTOs;
using BuildCore.WorkflowEngine.Application.Interfaces;
using BuildCore.WorkflowEngine.Domain.Entities;
using BuildCore.WorkflowEngine.Domain.Interfaces;

namespace BuildCore.WorkflowEngine.Application.UseCases;

/// <summary>
/// İş akışı servisi implementasyonu
/// </summary>
public class WorkflowService : IWorkflowService
{
    private readonly IWorkflowDefinitionRepository _workflowDefinitionRepository;
    private readonly IWorkflowInstanceRepository _workflowInstanceRepository;
    private readonly IUnitOfWork _unitOfWork;

    public WorkflowService(
        IWorkflowDefinitionRepository workflowDefinitionRepository,
        IWorkflowInstanceRepository workflowInstanceRepository,
        IUnitOfWork unitOfWork)
    {
        _workflowDefinitionRepository = workflowDefinitionRepository;
        _workflowInstanceRepository = workflowInstanceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<WorkflowDefinitionDto> CreateWorkflowDefinitionAsync(
        CreateWorkflowDefinitionDto dto,
        CancellationToken cancellationToken = default)
    {
        var workflowDefinition = new WorkflowDefinition(
            dto.Name,
            dto.Description,
            dto.Version);

        await _workflowDefinitionRepository.AddAsync(workflowDefinition, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToWorkflowDefinitionDto(workflowDefinition);
    }

    public async Task<WorkflowDefinitionDto?> GetWorkflowDefinitionByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var workflowDefinition = await _workflowDefinitionRepository.GetByIdAsync(id, cancellationToken);
        return workflowDefinition != null ? MapToWorkflowDefinitionDto(workflowDefinition) : null;
    }

    public async Task<IEnumerable<WorkflowDefinitionDto>> GetAllWorkflowDefinitionsAsync(
        CancellationToken cancellationToken = default)
    {
        var workflowDefinitions = await _workflowDefinitionRepository.GetAllAsync(cancellationToken);
        return workflowDefinitions.Select(MapToWorkflowDefinitionDto);
    }

    public async Task<WorkflowDefinitionDto> UpdateWorkflowDefinitionAsync(
        Guid id,
        CreateWorkflowDefinitionDto dto,
        CancellationToken cancellationToken = default)
    {
        var workflowDefinition = await _workflowDefinitionRepository.GetByIdAsync(id, cancellationToken);
        if (workflowDefinition == null)
            throw new KeyNotFoundException($"İş akışı tanımı bulunamadı. Id: {id}");

        workflowDefinition.Update(dto.Name, dto.Description);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToWorkflowDefinitionDto(workflowDefinition);
    }

    public async Task DeleteWorkflowDefinitionAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var workflowDefinition = await _workflowDefinitionRepository.GetByIdAsync(id, cancellationToken);
        if (workflowDefinition == null)
            throw new KeyNotFoundException($"İş akışı tanımı bulunamadı. Id: {id}");

        await _workflowDefinitionRepository.DeleteAsync(id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<WorkflowInstanceDto> StartWorkflowInstanceAsync(
        StartWorkflowInstanceDto dto,
        CancellationToken cancellationToken = default)
    {
        var workflowDefinition = await _workflowDefinitionRepository.GetByIdAsync(
            dto.WorkflowDefinitionId,
            cancellationToken);

        if (workflowDefinition == null)
            throw new KeyNotFoundException($"İş akışı tanımı bulunamadı. Id: {dto.WorkflowDefinitionId}");

        if (!workflowDefinition.IsActive)
            throw new InvalidOperationException("İş akışı tanımı aktif değil.");

        var workflowInstance = new WorkflowInstance(
            dto.WorkflowDefinitionId,
            dto.InitiatorUserId,
            dto.ContextData);

        // İlk adımı bul ve başlat
        var firstStep = workflowDefinition.Steps
            .OrderBy(s => s.Order)
            .FirstOrDefault();

        if (firstStep != null)
        {
            workflowInstance.Start(firstStep.Id);
        }
        else
        {
            workflowInstance.Start();
        }

        workflowInstance.AddHistoryEntry("Started", dto.InitiatorUserId);

        await _workflowInstanceRepository.AddAsync(workflowInstance, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToWorkflowInstanceDto(workflowInstance, workflowDefinition);
    }

    public async Task<WorkflowInstanceDto?> GetWorkflowInstanceByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var workflowInstance = await _workflowInstanceRepository.GetByIdAsync(id, cancellationToken);
        if (workflowInstance == null)
            return null;

        var workflowDefinition = await _workflowDefinitionRepository.GetByIdAsync(
            workflowInstance.WorkflowDefinitionId,
            cancellationToken);

        return MapToWorkflowInstanceDto(workflowInstance, workflowDefinition);
    }

    public async Task<IEnumerable<WorkflowInstanceDto>> GetWorkflowInstancesByDefinitionIdAsync(
        Guid workflowDefinitionId,
        CancellationToken cancellationToken = default)
    {
        var workflowInstances = await _workflowInstanceRepository.GetByWorkflowDefinitionIdAsync(
            workflowDefinitionId,
            cancellationToken);

        var workflowDefinition = await _workflowDefinitionRepository.GetByIdAsync(
            workflowDefinitionId,
            cancellationToken);

        return workflowInstances.Select(wi => MapToWorkflowInstanceDto(wi, workflowDefinition));
    }

    public async Task CompleteStepAsync(
        Guid workflowInstanceId,
        Guid stepId,
        string? userId = null,
        CancellationToken cancellationToken = default)
    {
        var workflowInstance = await _workflowInstanceRepository.GetByIdAsync(
            workflowInstanceId,
            cancellationToken);

        if (workflowInstance == null)
            throw new KeyNotFoundException($"İş akışı instance bulunamadı. Id: {workflowInstanceId}");

        if (workflowInstance.Status != "Running")
            throw new InvalidOperationException("Sadece çalışan iş akışları adım tamamlayabilir.");

        var workflowDefinition = await _workflowDefinitionRepository.GetByIdAsync(
            workflowInstance.WorkflowDefinitionId,
            cancellationToken);

        if (workflowDefinition == null)
            throw new KeyNotFoundException("İş akışı tanımı bulunamadı.");

        // Geçişleri kontrol et ve sonraki adıma geç
        var currentStep = workflowDefinition.Steps.FirstOrDefault(s => s.Id == stepId);
        if (currentStep == null)
            throw new KeyNotFoundException($"Adım bulunamadı. Id: {stepId}");

        var transitions = currentStep.OutgoingTransitions;
        var nextStep = transitions
            .OrderBy(t => t.ToStep.Order)
            .Select(t => workflowDefinition.Steps.FirstOrDefault(s => s.Id == t.ToStepId))
            .FirstOrDefault();

        workflowInstance.AddHistoryEntry("StepCompleted", userId, $"Step: {currentStep.Name}");

        if (nextStep != null)
        {
            workflowInstance.MoveToStep(nextStep.Id);
        }
        else
        {
            // Son adım, iş akışını tamamla
            workflowInstance.Complete();
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task CancelWorkflowInstanceAsync(
        Guid workflowInstanceId,
        string? userId = null,
        CancellationToken cancellationToken = default)
    {
        var workflowInstance = await _workflowInstanceRepository.GetByIdAsync(
            workflowInstanceId,
            cancellationToken);

        if (workflowInstance == null)
            throw new KeyNotFoundException($"İş akışı instance bulunamadı. Id: {workflowInstanceId}");

        workflowInstance.Cancel();
        workflowInstance.AddHistoryEntry("Cancelled", userId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static WorkflowDefinitionDto MapToWorkflowDefinitionDto(WorkflowDefinition workflowDefinition)
    {
        return new WorkflowDefinitionDto
        {
            Id = workflowDefinition.Id,
            Name = workflowDefinition.Name,
            Description = workflowDefinition.Description,
            Version = workflowDefinition.Version,
            IsActive = workflowDefinition.IsActive,
            CreatedAt = workflowDefinition.CreatedAt,
            UpdatedAt = workflowDefinition.UpdatedAt,
            Steps = workflowDefinition.Steps
                .OrderBy(s => s.Order)
                .Select(s => new WorkflowStepDto
                {
                    Id = s.Id,
                    WorkflowDefinitionId = s.WorkflowDefinitionId,
                    Name = s.Name,
                    StepType = s.StepType,
                    Order = s.Order,
                    AssigneeRole = s.AssigneeRole,
                    AssigneeUserId = s.AssigneeUserId,
                    IsRequired = s.IsRequired,
                    TimeoutMinutes = s.TimeoutMinutes
                })
                .ToList()
        };
    }

    private static WorkflowInstanceDto MapToWorkflowInstanceDto(
        WorkflowInstance workflowInstance,
        WorkflowDefinition? workflowDefinition = null)
    {
        var currentStep = workflowDefinition?.Steps
            .FirstOrDefault(s => s.Id == workflowInstance.CurrentStepId);

        return new WorkflowInstanceDto
        {
            Id = workflowInstance.Id,
            WorkflowDefinitionId = workflowInstance.WorkflowDefinitionId,
            WorkflowDefinitionName = workflowDefinition?.Name ?? string.Empty,
            Status = workflowInstance.Status,
            CurrentStepId = workflowInstance.CurrentStepId,
            CurrentStepName = currentStep?.Name,
            InitiatorUserId = workflowInstance.InitiatorUserId,
            ContextData = workflowInstance.ContextData,
            StartedAt = workflowInstance.StartedAt,
            CompletedAt = workflowInstance.CompletedAt,
            CreatedAt = workflowInstance.CreatedAt,
            History = workflowInstance.History
                .OrderBy(h => h.ActionDate)
                .Select(h => new WorkflowInstanceHistoryDto
                {
                    Id = h.Id,
                    WorkflowInstanceId = h.WorkflowInstanceId,
                    Action = h.Action,
                    UserId = h.UserId,
                    Notes = h.Notes,
                    ActionDate = h.ActionDate
                })
                .ToList()
        };
    }
}
