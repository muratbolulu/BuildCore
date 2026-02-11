namespace BuildCore.HumanResources.Application.DTOs;

/// <summary>
/// Token DTO'su
/// </summary>
public class TokenDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string TokenType { get; set; } = "Bearer";
    public DateTime ExpiresAt { get; set; }
    public UserDto User { get; set; } = null!;
}
