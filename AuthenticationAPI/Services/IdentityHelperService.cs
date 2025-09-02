using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AuthenticationAPI.Models.Configurations;
using AuthenticationAPI.Models.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationAPI.Services;

public class IdentityHelperService(AuthConfiguration authConfiguration) : IIdentityHelperService
{
    public string HashPassword(string password)
    {
        // Using the default Password Hasher Implementation (does not use the supplied IUser
        //  type properties for hashing). So using an empty string here.  
        string passwordHash = new PasswordHasher<string>().HashPassword(string.Empty, password);
        return passwordHash;
    }

    public bool VerifyPassword(string passwordHash, string password)
    {
        // Same as in the HashPassword Method.
        return new PasswordHasher<string>()
        .VerifyHashedPassword(string.Empty, passwordHash, password) == PasswordVerificationResult.Success;
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
        string token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        DateTime expiry = DateTime.UtcNow
                                .AddDays(authConfiguration.RefreshTokenConfig.ExpiryDays)
                                .AddHours(authConfiguration.RefreshTokenConfig.ExpiryHours)
                                .AddMinutes(authConfiguration.RefreshTokenConfig.ExpiryMinutes);

        return new RefreshTokenDto()
        {
            RefreshToken = token,
            ExpiryOn = expiry,
        };
    }

}
