using Microsoft.AspNetCore.Identity;

namespace AuthenticationAPI.Services;

public class PasswordHelper : IPasswordHelper
{
    public string HashPassword(string password)
    {
        // Using the default Password Hasher Implementation (does not use the supplied IUser
        //  type properties for hashing). So using an empty string here.  
        string passwordHash = new PasswordHasher<string>().HashPassword(string.Empty, password);
        return passwordHash;
    }

    public bool VerifyPassword(string passwordHash, string password)
    {
        // Same as in the HashPassword Method.
        return new PasswordHasher<string>()
            .VerifyHashedPassword(string.Empty, passwordHash, password) == PasswordVerificationResult.Success;
    }
}