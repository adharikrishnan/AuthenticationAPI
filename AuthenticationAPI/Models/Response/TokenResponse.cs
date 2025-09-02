using System;
using System.Text.Json.Serialization;
using AuthenticationAPI.Models.Dtos;

namespace AuthenticationAPI.Models.Response;

public class TokenResponse(AccessTokenDto accessTokenDto, RefreshTokenDto refreshTokenDto)
{
    /// <summary>
    /// The Access Token issued to the user after successful authentication.
    /// </summary>
    public string AccessToken { get; init; } = accessTokenDto.AccessToken;

    /// <summary>
    /// The time (in seconds) until the access token expires.
    /// </summary>
    public DateTime AccessTokenExpiry { get; init; } = accessTokenDto.ExpiryOn;

    /// <summary>
    /// The Refresh Token issued to the user to obtain a new access token after the current one expires.
    /// </summary>
    public string RefreshToken { get; init; } = refreshTokenDto.RefreshToken;

    /// <summary>
    /// The time (in seconds) until the refresh token expires.
    /// </summary>
    public DateTime RefreshTokenExpiry { get; init; } = refreshTokenDto.ExpiryOn;
}
