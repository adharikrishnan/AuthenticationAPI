using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationAPI.Entities;

[Index(nameof(Username), IsUnique = true)]
public class User
{
    public int Id { get; init; }

    public required string Username { get; init; }
    
    public string? Email { get; init; }

    public required string PasswordHash { get; init; }

    public int RoleId { get; init; }

    public Role Role { get; init; } = null!;

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public string CreatedBy { get; init; } = "admin";

    public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;

    public string UpdatedBy { get; init; } = "admin";

}
