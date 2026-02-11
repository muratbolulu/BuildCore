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
        // HasQueryFilter otomatik olarak uygulanır, ama explicit filter ekliyoruz
        return await _dbSet
            .Where(u => !u.IsDeleted && u.Email == email)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, Guid? excludeUserId = null, CancellationToken cancellationToken = default)
    {
        // HasQueryFilter otomatik olarak uygulanır, ama explicit filter ekliyoruz
        var query = _dbSet.Where(u => !u.IsDeleted && u.Email == email);
        
        if (excludeUserId.HasValue)
        {
            query = query.Where(u => u.Id != excludeUserId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public override async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // HasQueryFilter otomatik olarak Include'larla birlikte çalışır
        return await _dbSet
            .Where(u => !u.IsDeleted && u.Id == id)
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public override async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        // HasQueryFilter otomatik olarak Include'larla birlikte çalışır
        // IsDeleted = false olan kullanıcıları getirir
        return await _dbSet
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Where(u => !u.IsDeleted) // Explicit filter (HasQueryFilter zaten var ama ekstra güvenlik için)
            .ToListAsync(cancellationToken);
    }
}

