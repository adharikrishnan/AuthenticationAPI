using AuthenticationAPI.DataAccess;
using AuthenticationAPI.Models.Dtos;
using AuthenticationAPI.Models.Requests;
using AuthenticationAPI.Mappers;
using AuthenticationAPI.Models.Common;
using AuthenticationAPI.Models.Response;

namespace AuthenticationAPI.Services;

public class AuthService(AuthDbContext dbContext, IIdentityHelperService identityHelperService) : IAuthService
{
    public async Task<Result> RegisterUserAsync(AddUserRequest request, CancellationToken ct)
    {
        if (await dbContext.UserExistsAsync(request.Username, ct))
        {
            return new Result();
        }
        string hashedPassword = identityHelperService.HashPassword(request.Password);
        await dbContext.AddUserAsync(request.ToUser(hashedPassword), ct);
        
        return new Result();
    }

    public async Task<TokenResponse> HandleTokenRequest(TokenRequest request, CancellationToken ct)
    {
        UserDto? user = await dbContext.GetUserByUsernameAsync(request.Username, ct);
        
        var refreshTokenDto = identityHelperService.GenerateRefreshToken();
        refreshTokenDto.UserId = user.UserId;
        
        await dbContext.AddRefreshTokenAsync(refreshTokenDto.ToRefreshToken(), ct);
        
        var accessTokenDto = identityHelperService.GenerateJwtToken(user);
        
        return new TokenResponse()
        {
            AccessToken = accessTokenDto.AccessToken,
            AccessTokenExpiry = accessTokenDto.ExpiryOn,
            RefreshToken = refreshTokenDto.RefreshToken,
            RefreshTokenExpiry = refreshTokenDto.ExpiryOn,
        };
    }

    public bool VerifyPassword(UserDto user, TokenRequest request)
                => identityHelperService.VerifyPassword(user.PasswordHash, request.Password);

    /*public async Task <ServiceResponse<TokenResponse>> HandleRefreshTokenRequest(RefreshTokenRequest request, CancellationToken ct)
    {
        RefreshToken? refreshTokenDetails = await dbContext.GetRefreshTokenDataAsync(request.RefreshToken, ct);

        if (refreshTokenDetails is null)
        {
            return new ServiceResponse<TokenResponse>(HttpStatusCode.Unauthorized, "Refresh token invalid.", true, null);
        }
        await dbContext.UpdateRefreshTokenAsync(refreshTokenDetails, ct);

        
        return await HandleTokenRequest(refreshTokenDetails.User.ToUserDto(), ct);
    }*/

}
