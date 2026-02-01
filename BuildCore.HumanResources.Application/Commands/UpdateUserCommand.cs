using BuildCore.HumanResources.Application.DTOs;

namespace BuildCore.HumanResources.Application.Commands;

/// <summary>
/// Kullanıcı güncelleme komutu
/// </summary>
public class UpdateUserCommand
{
    public Guid Id { get; set; }
    public UpdateUserDto UserDto { get; set; } = null!;
}

