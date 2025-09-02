namespace AuthenticationAPI.Models.Dtos;

public class AccessTokenDto
{
    public required string AccessToken { get; init; }
    
    public required DateTime ExpiryOn { get; init; }
}