using BuildCore.HumanResources.Application.Interfaces;
using BuildCore.HumanResources.Infrastructure.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildCore.HumanResources.Infrastructure.Authentication;

/// <summary>
/// Authentication servisleri için extension metodlar
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuthenticationServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Password hasher
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        // Token service
        services.AddScoped<ITokenService, JwtTokenService>();

        return services;
    }
}
