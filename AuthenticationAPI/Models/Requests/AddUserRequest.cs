using AuthenticationAPI.Enums;

namespace AuthenticationAPI.Models.Requests;

public sealed record class AddUserRequest
{
    public required string Username { get; init; }
    
    public string? Email { get; init; }

    public required string Password { get; init; }
    
    public RoleType Role { get; init; } = RoleType.User;
}
