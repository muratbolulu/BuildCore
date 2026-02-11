using BuildCore.SharedKernel.Entities;

namespace BuildCore.HumanResources.Domain.Entities;

/// <summary>
/// Rol entity'si
/// </summary>
public class Role : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string NormalizedName { get; private set; } = string.Empty;

    // Navigation property
    public ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();

    // Private constructor for EF Core
    private Role() { }

    public Role(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Rol adı boş olamaz", nameof(name));

        Name = name;
        NormalizedName = name.ToUpperInvariant();
        Description = description;
    }

    public void Update(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Rol adı boş olamaz", nameof(name));

        Name = name;
        NormalizedName = name.ToUpperInvariant();
        Description = description;
    }

    public void AddUser(User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        if (!UserRoles.Any(ur => ur.UserId == user.Id))
        {
            UserRoles.Add(new UserRole(user.Id, Id));
        }
    }

    public void RemoveUser(Guid userId)
    {
        var userRole = UserRoles.FirstOrDefault(ur => ur.UserId == userId);
        if (userRole != null)
        {
            UserRoles.Remove(userRole);
        }
    }
}
