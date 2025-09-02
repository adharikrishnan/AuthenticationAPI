using AuthenticationAPI.Models.Dtos;

namespace AuthenticationAPI.Services;

public interface ITokenHelper
{
    AccessTokenDto GenerateJwtToken(UserDto user);

    RefreshTokenDto GenerateRefreshToken();
    
    void UpdateRefreshToken(RefreshTokenDto refreshToken);
}
