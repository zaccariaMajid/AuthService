using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Domain.Common;
using AuthService.Domain.Events;
using AuthService.Domain.Exceptions;
using AuthService.Domain.ValueObjects;
using src.BuldingBlocks.Domain;

namespace AuthService.Domain.AggregateRoots;

public class User : AggregateRoot<Guid>
{
    public Email Email { get; private set; } = null!;
    public PasswordHash PasswordHash { get; private set; } = null!;
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public bool IsActive { get; private set; }

    public List<Role> Roles { get; private set; } = new();

    private User() { }

    private User(Email email, PasswordHash passwordHash, string firstName, string lastName)
    {
        Id = Guid.NewGuid();
        Email = email;
        PasswordHash = passwordHash;
        FirstName = firstName;
        LastName = lastName;
        IsActive = false;

        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;

        AddDomainEvent(new UserRegistered(Id, Email.Value));
    }

    public static User Create(Email email, PasswordHash passwordHash, string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainException("First name cannot be empty.");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainException("Last name cannot be empty.");

        return new User(email, passwordHash, firstName, lastName);
    }

    public void ChangePassword(PasswordHash newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        Touch();
        AddDomainEvent(new UserPasswordChanged(Id, this.Email.Value));
    }

    public void Activate()
    {
        if (!IsActive)
        {
            IsActive = true;
            Touch();
            AddDomainEvent(new UserActivated(Id, Email.Value));
        }
    }
    
    public void AssignRole(Role role)
    {
        if (!Roles.Any(r => r.Id == role.Id))
        {
            Roles.Add(role);
            Touch();
            AddDomainEvent(new RoleAssignedToUser(Id, role.Id));
        }
    }

    public void RemoveRole(Role role)
    {
        Roles.RemoveAll(r => r.Id == role.Id);
        Touch();
    }
}
