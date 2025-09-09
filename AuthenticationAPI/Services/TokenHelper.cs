using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AuthenticationAPI.Models.Configurations;
using AuthenticationAPI.Models.Dtos;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationAPI.Services;

public class TokenHelper(IOptions<AuthConfiguration> authConfiguration) : ITokenHelper
{ 
    private readonly AuthConfiguration _authConfiguration = authConfiguration.Value;
    
    private (string Token, DateTime Expiry) GenerateTokenAndExpiry()
    {
        string token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        DateTime expiry = DateTime.UtcNow
            .AddDays(_authConfiguration.RefreshTokenConfig.ExpiryDays)
            .AddHours(_authConfiguration.RefreshTokenConfig.ExpiryHours)
            .AddMinutes(_authConfiguration.RefreshTokenConfig.ExpiryMinutes);

        return (token, expiry);
    }

    public AccessTokenDto GenerateJwtToken(UserDto user)
    {
        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authConfiguration.SecretKey));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        DateTime expiry = DateTime.UtcNow.AddMinutes(_authConfiguration.TokenExpiryMinutes);

        var tokenObj = new JwtSecurityToken(
            issuer: _authConfiguration.Issuer,
            audience: _authConfiguration.Audience,
            claims: claims,
            expires: expiry,
            signingCredentials: creds
        );

        var token = new JwtSecurityTokenHandler().WriteToken(tokenObj);
        
        return new AccessTokenDto()
        {
            AccessToken = token,
            ExpiryOn = expiry
        };
    }

    public RefreshTokenDto GenerateRefreshToken()
    {
        var (token , expiry) = GenerateTokenAndExpiry();
        return new RefreshTokenDto()
        {
            RefreshToken = token,
            ExpiryOn = expiry,
        };
    }

    public void UpdateRefreshToken(RefreshTokenDto refreshToken)
    {
        var (token , expiry) = GenerateTokenAndExpiry();
        refreshToken.RefreshToken = token;
        refreshToken.ExpiryOn = expiry;
    }

}
