namespace BuildCore.HumanResources.Application.Interfaces;

/// <summary>
/// Şifre hashleme servis interface'i
/// </summary>
public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}
