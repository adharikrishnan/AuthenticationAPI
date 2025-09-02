using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthenticationAPI.Entities;

public class Role
{
    public int Id { get; init; }

    public required string Name { get; init; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public string CreatedBy { get; init; } = "System";

    public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;

    public string UpdatedBy { get; init; } = "System";
}
