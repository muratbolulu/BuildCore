using BuildCore.HumanResources.Application.DTOs;

namespace BuildCore.HumanResources.Application.Interfaces;

/// <summary>
/// Authentication servis interface'i
/// </summary>
public interface IAuthenticationService
{
    Task<TokenDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default);
    Task<UserDto> RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken = default);
}
