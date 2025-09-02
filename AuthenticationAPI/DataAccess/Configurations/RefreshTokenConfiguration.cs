using AuthenticationAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthenticationAPI.DataAccess.Configurations;

public class RefreshTokenConfiguration :  IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        
        builder.Property(x => x.Token).HasMaxLength(256).IsRequired();
        
        builder.HasIndex(x => x.Token).IsUnique();
        
        builder.Property(x => x.ExpiresOn)
            .HasConversion(x => x, x => DateTime.SpecifyKind(x, DateTimeKind.Utc));
        
        builder.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);    
        
        CommonEntityConfiguration.SetAuditFields(builder);
    }
}