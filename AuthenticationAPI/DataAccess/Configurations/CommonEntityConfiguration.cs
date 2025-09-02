using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthenticationAPI.DataAccess.Configurations;

public static class CommonEntityConfiguration
{
    public static void SetAuditFields<TEntity>(EntityTypeBuilder<TEntity> builder) 
        where TEntity : class
    {
        builder.Property<string>("CreatedBy").HasMaxLength(100).IsRequired();
        builder.Property<DateTime>("CreatedAt").IsRequired();
        builder.Property<DateTime>("CreatedAt")
            .HasConversion(x => x, x => DateTime.SpecifyKind(x, DateTimeKind.Utc));
        
        builder.Property<string>("UpdatedBy").HasMaxLength(100);
        builder.Property<DateTime>("UpdatedAt").IsRequired();
        builder.Property<DateTime>("UpdatedAt")
            .HasConversion(x => x, x => DateTime.SpecifyKind(x, DateTimeKind.Utc));
    }
}