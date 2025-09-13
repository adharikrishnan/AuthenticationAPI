using AuthenticationAPI.Extensions;
using AuthenticationAPI.Models.Requests;
using AuthenticationAPI.Services;
using AuthenticationAPI.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(IValidatorFactory validatorFactory, IAuthService authService) : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(AddUserRequest request, CancellationToken ct)
        {
            var validationResult = await validatorFactory.HandleValidationAsync(request, ct);

            if (!validationResult.IsValid)
                return validationResult.MatchValidationError();

            var result = await authService.RegisterUserAsync(request, ct);
            return result.MatchApiResponse("User Created.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser(TokenRequest request, CancellationToken ct)
        {
            var validationResult = await validatorFactory.HandleValidationAsync(request, ct);

            if (!validationResult.IsValid)
                return validationResult.MatchValidationError();

            var result = await authService.HandleTokenRequest(request, ct);
            return result.MatchApiResponse();
        }

        [HttpPost("login/refresh-token")]
        public async Task<IActionResult> LoginWithRefreshToken(RefreshTokenRequest request, CancellationToken ct)
        {
            var validationResult = await validatorFactory.HandleValidationAsync(request, ct);

            if (!validationResult.IsValid)
                return validationResult.MatchValidationError();

            var result = await authService.HandleRefreshTokenRequest(request, ct);
            return result.MatchApiResponse();
        }

        [Authorize]
        [HttpPut("login/revoke-token")]
        public async Task<IActionResult> RevokeRefreshToken(RefreshTokenRequest request, CancellationToken ct)
        {
            var validationResult = await validatorFactory.HandleValidationAsync(request, ct);

            if (!validationResult.IsValid)
                return validationResult.MatchValidationError();

            var result = await authService.RevokeRefreshToken(request, ct);
            return result.MatchApiResponse("Refresh Token Revoked.");
        }

        [HttpGet("HealthCheck")]
        public IActionResult HealthCheck()
        {
            bool dbConnectionStatus =  authService.CheckDbConnection();
            return Ok($"Service is running. Database connection status: " + (dbConnectionStatus ? "Connected" : "Not Connected"));
        }

    }
}
