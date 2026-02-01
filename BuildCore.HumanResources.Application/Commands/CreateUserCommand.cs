using BuildCore.HumanResources.Application.DTOs;

namespace BuildCore.HumanResources.Application.Commands;

/// <summary>
/// Kullanıcı oluşturma komutu
/// </summary>
public class CreateUserCommand
{
    public CreateUserDto UserDto { get; set; } = null!;
}

