using System;
using System.Text.Json.Serialization;

namespace AuthenticationAPI.Models.Response;

public class TokenResponse
{
    /// <summary>
    /// The Access Token issued to the user after successful authentication.
    /// </summary>
    /*[JsonPropertyName("accessToken")]*/
    public required string AccessToken { get; set; }
    
    /// <summary>
    /// The time (in seconds) until the access token expires.
    /// </summary>
    /*[JsonPropertyName("accessTokenExpiry")]*/
    public DateTime AccessTokenExpiry { get; set; } 

    /// <summary>
    /// The Refresh Token issued to the user to obtain a new access token after the current one expires.
    /// </summary>
    /*[JsonPropertyName("refreshToken")]*/
    public required string RefreshToken { get; set; }
    
    /// <summary>
    /// The time (in seconds) until the refresh token expires.
    /// </summary>
    /*[JsonPropertyName("refreshTokenExpiry")]*/
    public DateTime RefreshTokenExpiry { get; set; }

}
