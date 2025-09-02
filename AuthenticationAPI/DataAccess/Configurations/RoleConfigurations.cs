using AuthenticationAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthenticationAPI.DataAccess.Configurations;

public class RoleConfigurations : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        
        builder.HasIndex(x => x.Name).IsUnique();
        builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
        
        CommonEntityConfiguration.SetAuditFields(builder);
        
        builder.HasData(
            new Role
            {
                Id = 1,
                Name = "User",
                CreatedAt = new DateTime(2025, 8, 29, 12, 30, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2025, 8, 29, 12, 30, 0, DateTimeKind.Utc)
            },
            new Role
            {
                Id = 2,
                Name = "Admin",
                CreatedAt = new DateTime(2025, 8, 29, 12, 30, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2025, 8, 29, 12, 30, 0, DateTimeKind.Utc)
            }
        );
    }
}