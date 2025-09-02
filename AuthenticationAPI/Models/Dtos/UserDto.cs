using AuthenticationAPI.Enums;

namespace AuthenticationAPI.Models.Dtos;

public class UserDto
{
    public int UserId { get; init; }
    
    public required string Username { get; init; }

    public string? Email { get; init; }

    public required string PasswordHash { get; init; }

    public RoleType Role { get; init; } = RoleType.User;
}
