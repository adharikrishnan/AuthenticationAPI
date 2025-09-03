namespace AuthenticationAPI.Models.Requests;

public sealed record TokenRequest
{
    public required string Username { get; set; }

    public required string Password { get; set; }
}
