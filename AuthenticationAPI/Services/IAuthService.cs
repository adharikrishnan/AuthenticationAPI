using AuthenticationAPI.Models.Common;
using AuthenticationAPI.Models.Dtos;
using AuthenticationAPI.Models.Requests;
using AuthenticationAPI.Models.Response;

namespace AuthenticationAPI.Services;

public interface IAuthService
{
    Task<Result> RegisterUserAsync(AddUserRequest request, CancellationToken ct);

    Task<TokenResponse> HandleTokenRequest(TokenRequest request, CancellationToken ct);

    bool VerifyPassword(UserDto user, TokenRequest request);
    
    /*Task<ServiceResponse<TokenResponse>> HandleRefreshTokenRequest(RefreshTokenRequest request, CancellationToken ct);*/
}
