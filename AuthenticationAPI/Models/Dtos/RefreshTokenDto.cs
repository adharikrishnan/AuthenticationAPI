namespace AuthenticationAPI.Models.Dtos;

public class RefreshTokenDto
{
    public required string RefreshToken { get; init; }
    
    public int UserId { get; set; }

    public required DateTime ExpiryOn { get; init; }
}