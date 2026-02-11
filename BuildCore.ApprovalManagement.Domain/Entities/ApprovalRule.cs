using BuildCore.SharedKernel.Entities;

namespace BuildCore.ApprovalManagement.Domain.Entities;

/// <summary>
/// Onay kuralı entity'si (Aggregate Root)
/// </summary>
public class ApprovalRule : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string RuleType { get; private set; } = string.Empty; // Leave, Expense, Transfer, Custom
    public bool IsActive { get; private set; }
    public string? Condition { get; private set; } // JSON condition expression

    // Navigation properties
    public ICollection<ApprovalChain> ApprovalChains { get; private set; } = new List<ApprovalChain>();
    public ICollection<ApprovalRequest> ApprovalRequests { get; private set; } = new List<ApprovalRequest>();

    // Private constructor for EF Core
    private ApprovalRule() { }

    public ApprovalRule(
        string name,
        string ruleType,
        string? description = null,
        string? condition = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Onay kuralı adı boş olamaz", nameof(name));

        if (string.IsNullOrWhiteSpace(ruleType))
            throw new ArgumentException("Kural tipi boş olamaz", nameof(ruleType));

        Name = name;
        RuleType = ruleType;
        Description = description;
        Condition = condition;
        IsActive = true;
    }

    public void Update(string name, string? description = null, string? condition = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Onay kuralı adı boş olamaz", nameof(name));

        Name = name;
        Description = description;
        Condition = condition;
    }

    public void AddApprovalChain(ApprovalChain chain)
    {
        if (chain == null)
            throw new ArgumentNullException(nameof(chain));

        ApprovalChains.Add(chain);
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}
