using AuthService.Domain.AggregateRoots;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Persistence.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired();

        builder.Property(t => t.Description)
            .IsRequired(false);

        builder.Property(t => t.IsActive)
            .IsRequired();

        builder.HasIndex(t => t.Name).IsUnique();

        builder.HasMany(t => t.Products)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "TenantProducts",
                right => right.HasOne<Product>()
                    .WithMany()
                    .HasForeignKey("ProductId")
                    .OnDelete(DeleteBehavior.Cascade),
                left => left.HasOne<Tenant>()
                    .WithMany()
                    .HasForeignKey("TenantId")
                    .OnDelete(DeleteBehavior.Cascade),
                join =>
                {
                    join.HasKey("TenantId", "ProductId");
                    join.ToTable("TenantProducts");
                });
    }
}
