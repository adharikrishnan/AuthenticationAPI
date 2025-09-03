using AuthenticationAPI.Models.Common;
using AuthenticationAPI.Models.Dtos;
using AuthenticationAPI.Models.Requests;
using AuthenticationAPI.Models.Response;

namespace AuthenticationAPI.Services;

public interface IAuthService
{
    Task<Result> RegisterUserAsync(AddUserRequest request, CancellationToken ct);

    Task<Result<TokenResponse>> HandleTokenRequest(TokenRequest request, CancellationToken ct);
    
    Task<Result<TokenResponse>> HandleRefreshTokenRequest(RefreshTokenRequest request, CancellationToken ct);
    
    Task<Result> RevokeRefreshToken(RefreshTokenRequest request, CancellationToken ct);
}
