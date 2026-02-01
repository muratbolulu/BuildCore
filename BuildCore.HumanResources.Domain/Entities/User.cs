using BuildCore.SharedKernel.Entities;

namespace BuildCore.HumanResources.Domain.Entities;

/// <summary>
/// Kullanıcı entity'si
/// </summary>
public class User : BaseEntity
{
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? PhoneNumber { get; private set; }
    public string? Department { get; private set; }
    public string? Position { get; private set; }
    public DateTime? HireDate { get; private set; }

    // Private constructor for EF Core
    private User() { }

    public User(string firstName, string lastName, string email, string? phoneNumber = null, string? department = null, string? position = null, DateTime? hireDate = null)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        Department = department;
        Position = position;
        HireDate = hireDate;
    }

    public void Update(string firstName, string lastName, string email, string? phoneNumber = null, string? department = null, string? position = null, DateTime? hireDate = null)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        Department = department;
        Position = position;
        HireDate = hireDate;
        UpdateTimestamp();
    }

    public string GetFullName() => $"{FirstName} {LastName}";
}

