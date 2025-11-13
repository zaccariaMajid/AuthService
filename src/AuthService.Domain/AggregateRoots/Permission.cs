using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Domain.Common;
using AuthService.Domain.Exceptions;
using src.BuldingBlocks.Domain;

namespace AuthService.Domain.AggregateRoots;

public sealed class Permission : AggregateRoot<Guid>
{
    public string Name { get; private set; }
    public string Description { get; private set; }

    public Permission(string name, string description) : base()
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description ?? string.Empty;
    }

    public static Permission Create(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Permission name cannot be null or empty.", nameof(name));

        return new Permission(name, description);
    }
}
