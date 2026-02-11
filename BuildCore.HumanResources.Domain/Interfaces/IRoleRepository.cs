using BuildCore.HumanResources.Domain.Entities;
using BuildCore.SharedKernel.Interfaces;

namespace BuildCore.HumanResources.Domain.Interfaces;

/// <summary>
/// Rol repository interface'i
/// </summary>
public interface IRoleRepository : IRepository<Role>
{
    Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<Role?> GetByIdWithUsersAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
    new Task<IEnumerable<Role>> GetAllAsync(CancellationToken cancellationToken = default);
}
