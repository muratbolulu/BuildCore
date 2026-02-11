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
    public DateOnly? HireDate { get; private set; }
    public string? PasswordHash { get; private set; }

    // Navigation property
    public ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();

    // Private constructor for EF Core
    private User() { }

    public User(
        string firstName,
        string lastName,
        string email,
        string? phoneNumber = null,
        string? department = null,
        string? position = null,
        DateOnly? hireDate = null,
        string? passwordHash = null)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        Department = department;
        Position = position;
        HireDate = hireDate;
        PasswordHash = passwordHash;
    }

    public void Update(
        string firstName,
        string lastName,
        string email,
        string? phoneNumber = null,
        string? department = null,
        string? position = null,
        DateOnly? hireDate = null)
    {
        SetIdentity(firstName, lastName, email);
        PhoneNumber = phoneNumber;
        Department = department;
        Position = position;
        HireDate = hireDate;
    }

    public string GetFullName() => $"{FirstName} {LastName}";

    public void SetPasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash boş olamaz", nameof(passwordHash));

        PasswordHash = passwordHash;
    }

    private void SetIdentity(string firstName, string lastName, string email)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("FirstName boş olamaz");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("LastName boş olamaz");

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email boş olamaz");

        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }
}

