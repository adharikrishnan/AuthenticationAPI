namespace AuthenticationAPI.Entities;

public class RefreshToken
{
    public int Id { get; init; }
    
    public required string Token { get; init; }
    
    public int UserId { get; init; }
    
    public User User { get; init; } = null!;
    
    public DateTime ExpiresOn { get; init; }
    
    public bool IsRevoked { get; set; } =  false;
        
    public DateTime CreatedAt { get; init; }
    
    public string CreatedBy { get; init; } = "System";
    
    public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;
    
    public string UpdatedBy { get; init; } = "System";
}