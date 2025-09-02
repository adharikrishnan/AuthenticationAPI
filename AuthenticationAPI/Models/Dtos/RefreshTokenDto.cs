namespace AuthenticationAPI.Models.Dtos;

public class RefreshTokenDto
{
    public int Id { get; init; }
    
    public required string RefreshToken { get; set; }
    
    public int UserId { get; set; }
    
    public UserDto? User { get; init; } 

    public required DateTime ExpiryOn { get; set; }
    
}