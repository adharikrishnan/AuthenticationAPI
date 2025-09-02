namespace AuthenticationAPI.Models.Requests;

public class TokenRequest
{
    public required string Username { get; set; }

    public required string Password { get; set; }
}
