using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Domain.AggregateRoots;
using AuthService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        // Email as ValueObject
        builder.Property(u => u.Email)
            .HasConversion(
                v => v.Value,
                v => Email.Create(v))
            .IsRequired();

        // PasswordHash as ValueObject
        builder.OwnsOne(u => u.PasswordHash, ph =>
        {
            ph.Property(p => p.Hash).HasColumnName("PasswordHash");
            ph.Property(p => p.Salt).HasColumnName("PasswordSalt");
        });

        builder.Property(u => u.FirstName)
            .IsRequired();

        builder.Property(u => u.LastName)
            .IsRequired();

        builder.Property(u => u.IsActive)
            .IsRequired();

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.Property(u => u.UpdatedAt)
            .IsRequired();

        // Roles
        builder.OwnsMany(u => u.Roles, role =>
        {
            role.WithOwner().HasForeignKey("UserId");
            role.Property<Guid>("Id");
            role.HasKey("Id");

            role.Property(r => r.Name)
                .HasColumnName("RoleName")
                .IsRequired();
        });
    }
}
