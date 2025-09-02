namespace AuthenticationAPI.Models.Configurations;

public record RefreshConfiguration(int ExpiryDays, int ExpiryHours, int ExpiryMinutes);

public class AuthConfiguration
{
    public required string SecretKey { get; init; }

    public required string Issuer { get; init; }

    public required string Audience { get; init; }

    public required int TokenExpiryMinutes { get; init; }
    
    public required RefreshConfiguration RefreshTokenConfig { get; init; }
}
