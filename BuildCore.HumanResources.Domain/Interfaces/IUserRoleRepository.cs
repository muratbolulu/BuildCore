using BuildCore.HumanResources.Domain.Entities;
using BuildCore.SharedKernel.Interfaces;

namespace BuildCore.HumanResources.Domain.Interfaces;

/// <summary>
/// Kullanıcı-Rol ilişki repository interface'i
/// </summary>
public interface IUserRoleRepository : IRepository<UserRole>
{
    Task<UserRole?> GetByUserAndRoleAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserRole>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserRole>> GetByRoleIdAsync(Guid roleId, CancellationToken cancellationToken = default);
    Task<int> GetUserRoleCountAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<int> GetRoleUserCountAsync(Guid roleId, CancellationToken cancellationToken = default);
}
