using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AuthenticationAPI.Models.Configurations;
using AuthenticationAPI.Models.Dtos;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationAPI.Services;

public class TokenHelper(AuthConfiguration authConfiguration) : ITokenHelper
{ 
    
    private (string Token, DateTime Expiry) GenerateTokenAndExpiry()
    {
        string token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        DateTime expiry = DateTime.UtcNow
            .AddDays(authConfiguration.RefreshTokenConfig.ExpiryDays)
            .AddHours(authConfiguration.RefreshTokenConfig.ExpiryHours)
            .AddMinutes(authConfiguration.RefreshTokenConfig.ExpiryMinutes);

        return (token, expiry);
    }

    public AccessTokenDto GenerateJwtToken(UserDto user)
    {
        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authConfiguration.SecretKey));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        DateTime expiry = DateTime.UtcNow.AddMinutes(authConfiguration.TokenExpiryMinutes);

        var tokenObj = new JwtSecurityToken(
            issuer: authConfiguration.Issuer,
            audience: authConfiguration.Audience,
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
