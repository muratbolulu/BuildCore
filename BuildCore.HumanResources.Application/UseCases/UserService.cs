using BuildCore.HumanResources.Application.DTOs;
using BuildCore.HumanResources.Application.Interfaces;
using BuildCore.HumanResources.Domain.Entities;
using BuildCore.HumanResources.Domain.Interfaces;
using BuildCore.SharedKernel.Interfaces;

namespace BuildCore.HumanResources.Application.UseCases;

/// <summary>
/// Kullanıcı servis implementasyonu
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto, CancellationToken cancellationToken = default)
    {
        // Email kontrolü
        if (await _userRepository.EmailExistsAsync(createUserDto.Email, null, cancellationToken))
        {
            throw new InvalidOperationException($"Email '{createUserDto.Email}' zaten kullanılıyor.");
        }

        var user = new User(
            createUserDto.FirstName,
            createUserDto.LastName,
            createUserDto.Email,
            createUserDto.PhoneNumber,
            createUserDto.Department,
            createUserDto.Position,
            createUserDto.HireDate
        );

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(user);
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        return user == null ? null : MapToDto(user);
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);
        return users.Select(MapToDto);
    }

    public async Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto updateUserDto, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user == null)
        {
            throw new KeyNotFoundException($"Kullanıcı bulunamadı. Id: {id}");
        }

        // Email kontrolü (kendi email'i hariç)
        if (await _userRepository.EmailExistsAsync(updateUserDto.Email, id, cancellationToken))
        {
            throw new InvalidOperationException($"Email '{updateUserDto.Email}' zaten kullanılıyor.");
        }

        user.Update(
            updateUserDto.FirstName,
            updateUserDto.LastName,
            updateUserDto.Email,
            updateUserDto.PhoneNumber,
            updateUserDto.Department,
            updateUserDto.Position,
            updateUserDto.HireDate
        );

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(user);
    }

    public async Task DeleteUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user == null)
        {
            throw new KeyNotFoundException($"Kullanıcı bulunamadı. Id: {id}");
        }

        await _userRepository.DeleteAsync(id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            FullName = user.GetFullName(),
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Department = user.Department,
            Position = user.Position,
            HireDate = user.HireDate,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}

