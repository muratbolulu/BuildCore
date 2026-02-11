using System.ComponentModel.DataAnnotations;

namespace BuildCore.HumanResources.Application.DTOs;

/// <summary>
/// Kullanıcı oluşturma DTO'su
/// </summary>
public class CreateUserDto
{
    [Required(ErrorMessage = "Ad alanı zorunludur.")]
    [StringLength(100, ErrorMessage = "Ad en fazla 100 karakter olabilir.")]
    [Display(Name = "Ad")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Soyad alanı zorunludur.")]
    [StringLength(100, ErrorMessage = "Soyad en fazla 100 karakter olabilir.")]
    [Display(Name = "Soyad")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "E-posta alanı zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
    [StringLength(200, ErrorMessage = "E-posta en fazla 200 karakter olabilir.")]
    [Display(Name = "E-posta")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
    [StringLength(20, ErrorMessage = "Telefon numarası en fazla 20 karakter olabilir.")]
    [Display(Name = "Telefon")]
    public string? PhoneNumber { get; set; }

    [StringLength(100, ErrorMessage = "Departman en fazla 100 karakter olabilir.")]
    [Display(Name = "Departman")]
    public string? Department { get; set; }

    [StringLength(100, ErrorMessage = "Pozisyon en fazla 100 karakter olabilir.")]
    [Display(Name = "Pozisyon")]
    public string? Position { get; set; }

    [Display(Name = "İşe Başlama Tarihi")]
    public DateOnly? HireDate { get; set; } //calendar date
}

