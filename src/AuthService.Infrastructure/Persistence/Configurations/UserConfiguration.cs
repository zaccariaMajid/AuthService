using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Domain.AggregateRoots;
using AuthService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Persistence.Configurations;

public class UserConfiguration :  IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Email)
                .HasConversion(e => e.Value, v => Email.Create(v))
                .IsRequired();

            builder.OwnsMany(x => x.Roles);

            builder.Property(x => x.IsActive)
                .IsRequired();
        }
    }
{
    
}
