using System.Net;
using AuthenticationAPI.Models.Dtos;
using AuthenticationAPI.Models.Requests;
using AuthenticationAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationAPI.Controllers
{
    public sealed record Response(HttpStatusCode StatusCode, string Message);
    
    [Route("api/auth")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(AddUserRequest request, CancellationToken ct)
        {
            var result = await authService.RegisterUserAsync(request, ct);

            return Ok(new Response(HttpStatusCode.Created, "User created successfully"));
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser(TokenRequest request, CancellationToken ct)
        {
            var token = await authService.HandleTokenRequest(request, ct);

            return Ok(token);
        }

        [HttpPost("login/refresh-token")]
        public async Task<IActionResult> LoginWithRefreshToken(RefreshTokenRequest request, CancellationToken ct)
        {
            return await Task.FromResult(Ok());
        }
        
        [HttpGet("Test")]
        public IActionResult TestEndpoint()
        {
            return Problem();
        }
    }
}
