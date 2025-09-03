using AuthenticationAPI.Entities;
using AuthenticationAPI.Enums;
using AuthenticationAPI.Models.Dtos;
using AuthenticationAPI.Mappers;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationAPI.DataAccess;

public class AuthDbContext(DbContextOptions<AuthDbContext> options) : DbContext(options)
{
    private DbSet<User> Users => Set<User>();
    private DbSet<Role> Roles => Set<Role>();
    private DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public async Task<bool> UserExistsAsync(string username, CancellationToken ct)
    {
        return await Users.AnyAsync(u => u.Username == username, ct);
    }

    public async Task<bool> AddUserAsync(User user, CancellationToken ct)
    {
        await Users.AddAsync(user, ct);
        var created = await SaveChangesAsync(ct);
        return created > 0;
    }

    public async Task<UserDto?> GetUserByUsernameAsync(string username, CancellationToken ct)
    {
        return await Users
                        .Where(u => u.Username == username)
                        .Select(u => new UserDto
                        {
                            UserId = u.Id,
                            Username = u.Username,
                            Email = u.Email,
                            PasswordHash = u.PasswordHash,
                            Role = Enum.Parse<RoleType>(u.Role.Name),
                        })
                        .FirstOrDefaultAsync(ct);
    }

    public async Task<bool> AddRefreshTokenAsync(RefreshToken refreshToken ,CancellationToken ct)
    {
        await RefreshTokens.AddAsync(refreshToken, ct);
        var created = await SaveChangesAsync(ct);
        return created > 0;
    }

    public async Task<RefreshTokenDto?> GetRefreshTokenDataAsync(string refreshToken, CancellationToken ct)
        => await RefreshTokens
            .Where(r => r.Token == refreshToken)
            .Select(r => new RefreshTokenDto
            {
                Id = r.Id,
                RefreshToken = r.Token,
                ExpiryOn = r.ExpiresOn,
                IsRevoked = r.IsRevoked,
                UserId = r.UserId,
                User = r.User.ToUserDto()
            })
            .FirstOrDefaultAsync(ct);

    public async Task<bool> UpdateRefreshTokenAsync(RefreshToken refreshToken, CancellationToken ct)
    {
        RefreshTokens.Attach(refreshToken);
        
        Entry(refreshToken).Property(r => r.Token).IsModified = true;
        Entry(refreshToken).Property(r => r.ExpiresOn).IsModified = true;
        Entry(refreshToken).Property(r => r.UpdatedBy).IsModified = true;
        Entry(refreshToken).Property(r => r.UpdatedAt).IsModified = true;
        
        return await SaveChangesAsync(ct) > 0;
    }

    public async Task<bool> RevokeRefreshToken(string refreshToken, CancellationToken ct)
    {
        var refreshTokenEntry = await RefreshTokens.AsTracking().FirstOrDefaultAsync(r => r.Token == refreshToken, ct);

        if (refreshTokenEntry != null) return false;

        refreshTokenEntry!.IsRevoked = true;
        
        return  await SaveChangesAsync(ct) > 0;
    }
}
