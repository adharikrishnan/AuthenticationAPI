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

        // adds a default admin user as the first user for testing purposes
        // not something to be used in production, just adding this for convenience
        // TODO: Create secure user creation and management process systematically
        // Username is admin
        // Password is "admin@2025"
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Username = "admin",
                Email = null,
                PasswordHash = "AQAAAAIAAYagAAAAEBFIfNPAOhEACrN97HkZI+2OUGGh/3Iaj/jhgBWBtD3wg33TeLw34dzgaJLIxjhfqw==", // admin@2025
                RoleId = (int)RoleType.Admin,
                CreatedAt = new DateTime(2025, 8, 29, 12, 30, 0, 0, DateTimeKind.Utc),
                CreatedBy = "System",
                UpdatedAt = new DateTime(2025, 8, 29, 12, 30, 0, 0, DateTimeKind.Utc),
                UpdatedBy = "System"
            });

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

        if (refreshTokenEntry is null) return false;

        refreshTokenEntry!.IsRevoked = true;
        
        return await SaveChangesAsync(ct) > 0;
    }

    public async Task<int> DeleteRefreshTokensAsync(CancellationToken ct)
    {
        return await RefreshTokens.Where(x => x.IsRevoked || x.ExpiresOn < DateTime.UtcNow).ExecuteDeleteAsync(ct);
    }
}
