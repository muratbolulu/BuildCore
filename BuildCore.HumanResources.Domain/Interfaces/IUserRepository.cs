using BuildCore.HumanResources.Domain.Entities;
using BuildCore.SharedKernel.Interfaces;

namespace BuildCore.HumanResources.Domain.Interfaces;

/// <summary>
/// Kullanıcı repository interface'i
/// </summary>
public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, Guid? excludeUserId = null, CancellationToken cancellationToken = default);
}

