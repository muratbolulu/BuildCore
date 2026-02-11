using BuildCore.SharedKernel.Entities;

namespace BuildCore.HumanResources.Domain.Entities;

/// <summary>
/// Kullanıcı-Rol ilişki entity'si (Many-to-Many)
/// </summary>
public class UserRole : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid RoleId { get; private set; }

    // Navigation properties
    public User User { get; private set; } = null!;
    public Role Role { get; private set; } = null!;

    // Private constructor for EF Core
    private UserRole() { }

    public UserRole(Guid userId, Guid roleId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId boş olamaz", nameof(userId));

        if (roleId == Guid.Empty)
            throw new ArgumentException("RoleId boş olamaz", nameof(roleId));

        UserId = userId;
        RoleId = roleId;
    }
}
