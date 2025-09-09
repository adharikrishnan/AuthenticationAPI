using AuthenticationAPI.DataAccess;
using AuthenticationAPI.Models.Dtos;
using AuthenticationAPI.Models.Requests;
using AuthenticationAPI.Mappers;
using AuthenticationAPI.Models.Common;
using AuthenticationAPI.Models.Response;

namespace AuthenticationAPI.Services;

public class AuthService(AuthDbContext dbContext, ITokenHelper tokenHelper, IPasswordHelper passwordHelper) : IAuthService
{
    public async Task<Result> RegisterUserAsync(AddUserRequest request, CancellationToken ct)
    {
        if (await dbContext.UserExistsAsync(request.Username.Trim(), ct))
        {
            return new Result
                ("USER_EXISTS", "User already exists.", ErrorType.Conflict);
        }
        string hashedPassword = passwordHelper.HashPassword(request.Password);
        
        await dbContext.AddUserAsync(request.ToUser(hashedPassword), ct);
        
        return new Result();
    }

    public async Task<Result<TokenResponse>> HandleTokenRequest(TokenRequest request, CancellationToken ct)
    {
        UserDto? user = await dbContext.GetUserByUsernameAsync(request.Username, ct);

        if (user is null)
            return new Result<TokenResponse>
                ("USER_NOT_FOUND", "User does not exist.", ErrorType.NotFound);


        if (!passwordHelper.VerifyPassword(user!.PasswordHash, request.Password))
            return new Result<TokenResponse>
                ("INVALID_PASSWORD" ,"Invalid Credentials.", ErrorType.Unauthorized);
        
        var refreshTokenDto = tokenHelper.GenerateRefreshToken();
        refreshTokenDto.UserId = user.UserId;
        
        await dbContext.AddRefreshTokenAsync(refreshTokenDto.ToRefreshToken(), ct);

        var accessTokenDto = tokenHelper.GenerateJwtToken(user);

        return new Result<TokenResponse>(new TokenResponse(accessTokenDto, refreshTokenDto));
    }

    public async Task <Result<TokenResponse>> HandleRefreshTokenRequest(RefreshTokenRequest request, CancellationToken ct)
    {
        var refreshTokenDto = await dbContext.GetRefreshTokenDataAsync(request.RefreshToken, ct);

        if (refreshTokenDto is null || refreshTokenDto.ExpiryOn < DateTime.UtcNow || refreshTokenDto.IsRevoked)
        {
            return new Result<TokenResponse>
                ("INVALID_REFRESH_TOKEN", "Invalid Token.", ErrorType.Unauthorized);
        }
        
        tokenHelper.UpdateRefreshToken(refreshTokenDto);
        
        await dbContext.UpdateRefreshTokenAsync(refreshTokenDto.ToRefreshToken(), ct);
        
        var accessTokenDto = tokenHelper.GenerateJwtToken(refreshTokenDto.User!);

        return new Result<TokenResponse>(new TokenResponse(accessTokenDto, refreshTokenDto));
    }

    public async Task<Result> RevokeRefreshToken(RefreshTokenRequest request, CancellationToken ct)
    {
        var revoked = await dbContext.RevokeRefreshToken(request.RefreshToken, ct);
        
        if(!revoked)
            return new Result
                ("INVALID_REFRESH_TOKEN", "Refresh Token is invalid.", ErrorType.NotFound);

        return new Result();
    }

    public async Task<int> DeleteInvalidRefreshTokensAsync(CancellationToken ct)
    {
        return await dbContext.DeleteRefreshTokensAsync(ct);
    }
}
