using AuthenticationAPI.Models.Dtos;

namespace AuthenticationAPI.Services;

public interface IIdentityHelperService
{
    public string HashPassword(string password);

    bool VerifyPassword(string passwordHash, string password);

    AccessTokenDto GenerateJwtToken(UserDto user);

    RefreshTokenDto GenerateRefreshToken();
}
