using BuildCore.HumanResources.Domain.Entities;
using BuildCore.HumanResources.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuildCore.HumanResources.Infrastructure.Persistence.Repositories;

/// <summary>
/// Rol repository implementasyonu
/// </summary>
public class RoleRepository : Repository<Role>, IRoleRepository
{
    public RoleRepository(HumanResourcesDbContext context) : base(context)
    {
    }

    public async Task<Role?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var normalizedName = name.ToUpperInvariant();
        return await _dbSet.FirstOrDefaultAsync(r => r.NormalizedName == normalizedName, cancellationToken);
    }

    public async Task<Role?> GetByIdWithUsersAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.UserRoles)
                .ThenInclude(ur => ur.User)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var normalizedName = name.ToUpperInvariant();
        return await _dbSet.AnyAsync(r => r.NormalizedName == normalizedName, cancellationToken);
    }

    public new async Task<IEnumerable<Role>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(r => r.UserRoles)
            .ToListAsync(cancellationToken);
    }
}
