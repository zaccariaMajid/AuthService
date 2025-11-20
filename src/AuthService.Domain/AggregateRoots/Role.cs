using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Domain.Common;
using AuthService.Domain.Events;
using AuthService.Domain.Exceptions;
using src.BuldingBlocks.Domain;

namespace AuthService.Domain.AggregateRoots;

public class Role : AggregateRoot<Guid>
{
    public string Name { get; private set; } = null!;
    public string Description { get; private set; } = null!;
    public ICollection<Permission>? Permissions { get; private set; } = new List<Permission>();

    private Role() : base() { }

    private Role(string name, string description, ICollection<Permission>? permissions) : base()
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description ?? string.Empty;
        Permissions = permissions;

        AddDomainEvent(new RoleCreated(Id, name));
    }

    public static Role Create(string name, string description, ICollection<Permission>? permissions = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Role name cannot be null or empty.", nameof(name));

        return new Role(name, description, permissions);
    }

    public void AddPermission(Permission permission)
    {
        if (permission is null)
            throw new DomainException("Permission Not Found", nameof(permission));

        if(Permissions is null)
            Permissions = new List<Permission>();

        if (!Permissions.Contains(permission))
        {
            Permissions.Add(permission);
            Touch();
            AddDomainEvent(new PermissionAddedToRole(Id, permission.Name));
        }
    }

    public void RemovePermission(Permission permission)
    {
        if (permission is null)
            throw new DomainException("Permission Not Found", nameof(permission));

        if (Permissions is not null && Permissions.Contains(permission))
        {
            Permissions.Remove(permission);
            if(Permissions.Count == 0)
                Permissions = null;

            Touch();
            AddDomainEvent(new PermissionRemovedFromRole(Id, permission.Name));
        }
    }
}
