using BuildCore.HumanResources.Application.DTOs;
using BuildCore.HumanResources.Application.Interfaces;
using BuildCore.HumanResources.Domain.Entities;
using BuildCore.HumanResources.Domain.Interfaces;
using BuildCore.SharedKernel.Interfaces;

namespace BuildCore.HumanResources.Application.UseCases;

/// <summary>
/// Rol servis implementasyonu
/// </summary>
public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RoleService(
        IRoleRepository roleRepository,
        IUserRepository userRepository,
        IUserRoleRepository userRoleRepository,
        IUnitOfWork unitOfWork)
    {
        _roleRepository = roleRepository;
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<RoleDto>> GetAllRolesAsync(CancellationToken cancellationToken = default)
    {
        var roles = await _roleRepository.GetAllAsync(cancellationToken);
        var roleDtos = new List<RoleDto>();
        foreach (var role in roles)
        {
            roleDtos.Add(await MapToDtoAsync(role, cancellationToken));
        }
        return roleDtos;
    }

    public async Task<RoleDto?> GetRoleByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var role = await _roleRepository.GetByIdAsync(id, cancellationToken);
        return role == null ? null : await MapToDtoAsync(role, cancellationToken);
    }

    public async Task<RoleDto> CreateRoleAsync(CreateRoleDto createRoleDto, CancellationToken cancellationToken = default)
    {
        // Rol adı kontrolü
        if (await _roleRepository.ExistsByNameAsync(createRoleDto.Name, cancellationToken))
        {
            throw new InvalidOperationException($"Rol '{createRoleDto.Name}' zaten mevcut.");
        }

        var role = new Role(createRoleDto.Name, createRoleDto.Description);
        await _roleRepository.AddAsync(role, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await MapToDtoAsync(role, cancellationToken);
    }

    public async Task<RoleDto> UpdateRoleAsync(Guid id, UpdateRoleDto updateRoleDto, CancellationToken cancellationToken = default)
    {
        var role = await _roleRepository.GetByIdAsync(id, cancellationToken);
        if (role == null)
        {
            throw new KeyNotFoundException($"Rol bulunamadı. Id: {id}");
        }

        // Rol adı kontrolü (kendi adı hariç)
        if (role.Name != updateRoleDto.Name && await _roleRepository.ExistsByNameAsync(updateRoleDto.Name, cancellationToken))
        {
            throw new InvalidOperationException($"Rol '{updateRoleDto.Name}' zaten mevcut.");
        }

        role.Update(updateRoleDto.Name, updateRoleDto.Description);
        await _roleRepository.UpdateAsync(role, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return await MapToDtoAsync(role, cancellationToken);
    }

    public async Task DeleteRoleAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var role = await _roleRepository.GetByIdAsync(id, cancellationToken);
        if (role == null)
        {
            throw new KeyNotFoundException($"Rol bulunamadı. Id: {id}");
        }

        // Eğer role atanmış kullanıcılar varsa silme işlemini engelle
        var userCount = await _userRoleRepository.GetRoleUserCountAsync(id, cancellationToken);
        if (userCount > 0)
        {
            throw new InvalidOperationException($"Bu role atanmış kullanıcılar bulunmaktadır. Önce kullanıcılardan rolü kaldırın.");
        }

        await _roleRepository.DeleteAsync(id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task AssignRoleToUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            throw new KeyNotFoundException($"Kullanıcı bulunamadı. Id: {userId}");
        }

        var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
        if (role == null)
        {
            throw new KeyNotFoundException($"Rol bulunamadı. Id: {roleId}");
        }

        // Kullanıcıya zaten bu rol atanmış mı kontrol et
        if (await _userRoleRepository.ExistsAsync(userId, roleId, cancellationToken))
        {
            throw new InvalidOperationException($"Kullanıcıya '{role.Name}' rolü zaten atanmış.");
        }

        // UserRole entity'sini oluştur ve ekle
        var userRole = new UserRole(userId, roleId);
        await _userRoleRepository.AddAsync(userRole, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveRoleFromUserAsync(Guid userId, Guid roleId, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            throw new KeyNotFoundException($"Kullanıcı bulunamadı. Id: {userId}");
        }

        var role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
        if (role == null)
        {
            throw new KeyNotFoundException($"Rol bulunamadı. Id: {roleId}");
        }

        // UserRole'i bul
        var userRole = await _userRoleRepository.GetByUserAndRoleAsync(userId, roleId, cancellationToken);
        if (userRole == null)
        {
            throw new InvalidOperationException($"Kullanıcıya '{role.Name}' rolü atanmamış.");
        }

        // UserRole'i sil (soft delete)
        await _userRoleRepository.DeleteAsync(userRole.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<RoleDto>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        // Kullanıcının varlığını kontrol et
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            throw new KeyNotFoundException($"Kullanıcı bulunamadı. Id: {userId}");
        }

        // UserRole'leri UserRoleRepository üzerinden al
        var userRoles = await _userRoleRepository.GetByUserIdAsync(userId, cancellationToken);
        return userRoles.Select(ur => MapToDto(ur.Role));
    }

    private async Task<RoleDto> MapToDtoAsync(Role role, CancellationToken cancellationToken = default)
    {
        var userCount = await _userRoleRepository.GetRoleUserCountAsync(role.Id, cancellationToken);
        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            CreatedAt = role.CreatedAt,
            UpdatedAt = role.UpdatedAt,
            UserCount = userCount
        };
    }

    private static RoleDto MapToDto(Role role)
    {
        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            CreatedAt = role.CreatedAt,
            UpdatedAt = role.UpdatedAt,
            UserCount = 0 // Bu metod sadece navigation property'den gelen roller için kullanılıyor
        };
    }
}
