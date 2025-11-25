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

        builder.OwnsMany(r => r.Permissions, perm =>
        {
            perm.WithOwner().HasForeignKey("RoleId");
            perm.Property<Guid>("Id");
            perm.HasKey("Id");
            
            perm.Property(p => p.Name)
                .HasColumnName("PermissionName")
                .IsRequired();

            perm.Property(p => p.Description)
                .HasColumnName("PermissionDescription");
        });
    }
}