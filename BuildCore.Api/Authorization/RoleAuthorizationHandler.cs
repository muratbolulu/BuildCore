using BuildCore.HumanResources.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BuildCore.Api.Authorization;

/// <summary>
/// Rol tabanlı authorization handler
/// </summary>
public class RoleAuthorizationHandler : AuthorizationHandler<RoleRequirement>
{
    private readonly IRoleService _roleService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RoleAuthorizationHandler(
        IRoleService roleService,
        IHttpContextAccessor httpContextAccessor)
    {
        _roleService = roleService;
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        RoleRequirement requirement)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            return;
        }

        // Kullanıcı ID'sini JWT token'dan al
        var userIdClaim = context.User.FindFirst("UserId") 
            ?? context.User.FindFirst(ClaimTypes.NameIdentifier);
        
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return;
        }

        // Kullanıcının rollerini al
        var userRoles = await _roleService.GetUserRolesAsync(userId);
        var userRoleNames = userRoles.Select(r => r.Name).ToList();

        // Gerekli rollerden herhangi birine sahip mi kontrol et
        if (requirement.Roles.Any(role => userRoleNames.Contains(role, StringComparer.OrdinalIgnoreCase)))
        {
            context.Succeed(requirement);
        }
    }
}

/// <summary>
/// Rol gereksinimi
/// </summary>
public class RoleRequirement : IAuthorizationRequirement
{
    public List<string> Roles { get; }

    public RoleRequirement(params string[] roles)
    {
        Roles = roles.ToList();
    }
}
