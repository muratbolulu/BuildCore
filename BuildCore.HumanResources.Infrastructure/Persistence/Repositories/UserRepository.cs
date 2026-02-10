using BuildCore.HumanResources.Domain.Entities;
using BuildCore.HumanResources.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuildCore.HumanResources.Infrastructure.Persistence.Repositories;

/// <summary>
/// Kullanıcı repository implementasyonu
/// </summary>
public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(HumanResourcesDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, Guid? excludeUserId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(u => u.Email == email);
        
        if (excludeUserId.HasValue)
        {
            query = query.Where(u => u.Id != excludeUserId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }
}

