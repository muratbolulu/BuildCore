using System.ComponentModel.DataAnnotations;

namespace BuildCore.HumanResources.Application.DTOs;

/// <summary>
/// Rol güncelleme DTO'su
/// </summary>
public class UpdateRoleDto
{
    [Required(ErrorMessage = "Rol adı zorunludur.")]
    [StringLength(100, ErrorMessage = "Rol adı en fazla 100 karakter olabilir.")]
    [Display(Name = "Rol Adı")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
    [Display(Name = "Açıklama")]
    public string? Description { get; set; }
}
