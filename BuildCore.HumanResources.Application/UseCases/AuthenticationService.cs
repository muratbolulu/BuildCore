using BuildCore.HumanResources.Application.DTOs;
using BuildCore.HumanResources.Application.Interfaces;
using BuildCore.HumanResources.Domain.Entities;
using BuildCore.HumanResources.Domain.Interfaces;
using BuildCore.SharedKernel.Interfaces;

namespace BuildCore.HumanResources.Application.UseCases;

/// <summary>
/// Authentication servis implementasyonu
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleService _roleService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IUnitOfWork _unitOfWork;

    public AuthenticationService(
        IUserRepository userRepository,
        IRoleService roleService,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _roleService = roleService;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _unitOfWork = unitOfWork;
    }

    public async Task<TokenDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default)
    {
        // Kullanıcıyı email ile bul
        var user = await _userRepository.GetByEmailAsync(loginDto.Email, cancellationToken);
        if (user == null)
        {
            throw new UnauthorizedAccessException("E-posta veya şifre hatalı.");
        }

        // Şifre kontrolü
        if (string.IsNullOrWhiteSpace(user.PasswordHash) || 
            !_passwordHasher.VerifyPassword(loginDto.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("E-posta veya şifre hatalı.");
        }

        // Kullanıcının rollerini al
        var roles = await _roleService.GetUserRolesAsync(user.Id, cancellationToken);

        // Token oluştur
        var token = _tokenService.GenerateToken(user, roles);

        // UserDto oluştur
        var userDto = new UserDto
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
            UpdatedAt = user.UpdatedAt,
            Roles = roles.ToList()
        };

        return new TokenDto
        {
            AccessToken = token,
            TokenType = "Bearer",
            ExpiresAt = _tokenService.GetTokenExpiration(),
            User = userDto
        };
    }

    public async Task<UserDto> RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken = default)
    {
        // Email kontrolü
        if (await _userRepository.EmailExistsAsync(registerDto.Email, null, cancellationToken))
        {
            throw new InvalidOperationException($"Email '{registerDto.Email}' zaten kullanılıyor.");
        }

        // Şifre hashleme
        var passwordHash = _passwordHasher.HashPassword(registerDto.Password);

        // Kullanıcı oluştur
        var user = new Domain.Entities.User(
            registerDto.FirstName,
            registerDto.LastName,
            registerDto.Email,
            registerDto.PhoneNumber,
            registerDto.Department,
            registerDto.Position,
            null,
            passwordHash
        );

        await _userRepository.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // UserDto oluştur
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
            UpdatedAt = user.UpdatedAt,
            Roles = new List<RoleDto>()
        };
    }
}
