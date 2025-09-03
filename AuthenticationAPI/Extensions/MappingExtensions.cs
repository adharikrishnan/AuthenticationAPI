using AuthenticationAPI.Entities;
using AuthenticationAPI.Enums;
using AuthenticationAPI.Models.Dtos;
using AuthenticationAPI.Models.Requests;

namespace AuthenticationAPI.Mappers;

public static class MappingExtensions
{
    public static User ToUser(this AddUserRequest request, string hashedPassword)
    {
        return new User()
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = hashedPassword,
            RoleId = (int)request.Role
        };
    }

    public static UserDto ToUserDto(this User user)
    {
        return new UserDto()
        {
            Username = user.Username,
            Email = user.Email,
            Role = (RoleType)user.RoleId,
            PasswordHash = user.PasswordHash,
        };
    }

    public static RefreshToken ToRefreshToken(this RefreshTokenDto refreshTokenDto, string? updatedBy = null)
    {
        return new RefreshToken()
        {
            Id = refreshTokenDto.Id,
            Token = refreshTokenDto.RefreshToken,
            UserId = refreshTokenDto.UserId,
            ExpiresOn = refreshTokenDto.ExpiryOn,
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = updatedBy ?? "System"
        };
    }
}
