namespace AuthenticationAPI.Services;

public interface IPasswordHelper
{
    string HashPassword(string password);

    bool VerifyPassword(string passwordHash, string password);
}