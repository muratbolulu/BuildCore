namespace BuildCore.HumanResources.Application.DTOs;

/// <summary>
/// Kullanıcı oluşturma DTO'su
/// </summary>
public class CreateUserDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? Department { get; set; }
    public string? Position { get; set; }
    public DateOnly? HireDate { get; set; } //calendar date
}

