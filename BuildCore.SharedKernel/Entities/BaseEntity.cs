namespace BuildCore.SharedKernel.Entities;

/// <summary>
/// Tüm entity'ler için base sınıf
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();

    public DateTimeOffset CreatedAt { get; protected set; }
    public DateTimeOffset? UpdatedAt { get; protected set; }

    public string CreatedBy { get; protected set; } = default!;
    public string? UpdatedBy { get; protected set; }

    public bool IsDeleted { get; protected set; }

    internal void SetCreated(string userId, DateTimeOffset now)
    {
        CreatedBy = userId;
        CreatedAt = now;
    }

    internal void SetUpdated(string userId, DateTimeOffset now)
    {
        UpdatedBy = userId;
        UpdatedAt = now;
    }

    public void MarkAsDeleted()
    {
        IsDeleted = true;
    }
}

