using System.Net;
using AuthenticationAPI.Extensions;
using AuthenticationAPI.Models.Requests;
using AuthenticationAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        /*[Authorize(Roles = "Admin")]*/
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(AddUserRequest request, CancellationToken ct)
        {
            var result = await authService.RegisterUserAsync(request, ct);
            return result.MatchApiResponse("User Created.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser(TokenRequest request, CancellationToken ct)
        {
            var result = await authService.HandleTokenRequest(request, ct);
            return result.MatchApiResponse();
        }

        [HttpPost("login/refresh-token")]
        public async Task<IActionResult> LoginWithRefreshToken(RefreshTokenRequest request, CancellationToken ct)
        {
            var result = await authService.HandleRefreshTokenRequest(request, ct);
            return result.MatchApiResponse();
        }
    }
}
