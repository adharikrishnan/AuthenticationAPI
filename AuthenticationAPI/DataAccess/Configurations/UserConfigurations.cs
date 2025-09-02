using AuthenticationAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthenticationAPI.DataAccess.Configurations;

public class UserConfigurations :  IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        
        builder.Property(x => x.Username).HasMaxLength(256).IsRequired();
        builder.HasIndex(x => x.Username).IsUnique();
        
        builder.Property(x => x.Email).HasMaxLength(256).IsRequired(false);
        builder.HasIndex(x => x.Email).IsUnique();
        
        builder.Property(x => x.PasswordHash).HasMaxLength(256).IsRequired();
        builder.HasIndex(x => x.PasswordHash).IsUnique();

        builder.HasOne(x => x.Role).WithOne().HasForeignKey<User>(x => x.RoleId);
        
        CommonEntityConfiguration.SetAuditFields(builder);
    }
}