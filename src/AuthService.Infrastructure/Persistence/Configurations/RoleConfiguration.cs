using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Domain.AggregateRoots;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name)
            .IsRequired();

        builder.Property(r => r.Description);

        builder.Property(r => r.TenantId)
            .IsRequired();
        builder.HasIndex(r => new { r.Name, r.TenantId }).IsUnique();
        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(r => r.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(r => r.Permissions)
            .WithMany()
            .UsingEntity<Dictionary<string, object>>(
                "RolePermissions",
                right => right.HasOne<Permission>()
                    .WithMany()
                    .HasForeignKey("PermissionId")
                    .OnDelete(DeleteBehavior.Cascade),
                left => left.HasOne<Role>()
                    .WithMany()
                    .HasForeignKey("RoleId")
                    .OnDelete(DeleteBehavior.Cascade),
                join =>
                {
                    join.HasKey("RoleId", "PermissionId");
                    join.ToTable("RolePermissions");
                });
    }
}
