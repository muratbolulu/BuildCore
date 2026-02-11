using BuildCore.HumanResources.Application.DTOs;
using BuildCore.HumanResources.Domain.Entities;

namespace BuildCore.HumanResources.Application.Interfaces;

/// <summary>
/// Token servis interface'i
/// </summary>
public interface ITokenService
{
    string GenerateToken(User user, IEnumerable<RoleDto> roles);
    DateTime GetTokenExpiration();
}
