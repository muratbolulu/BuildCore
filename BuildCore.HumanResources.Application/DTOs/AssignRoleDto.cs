using System.ComponentModel.DataAnnotations;

namespace BuildCore.HumanResources.Application.DTOs;

/// <summary>
/// Kullanıcıya rol atama DTO'su
/// </summary>
public class AssignRoleDto
{
    [Required(ErrorMessage = "Rol ID zorunludur.")]
    [Display(Name = "Rol ID")]
    public Guid RoleId { get; set; }
}
