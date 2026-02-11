namespace BuildCore.HumanResources.Application.DTOs;

/// <summary>
/// Rol DTO'su
/// </summary>
public class RoleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public int UserCount { get; set; }
}
