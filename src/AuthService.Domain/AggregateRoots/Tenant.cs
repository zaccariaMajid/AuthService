using AuthService.Domain.Common;
using AuthService.Domain.Exceptions;
using src.BuldingBlocks.Domain;

namespace AuthService.Domain.AggregateRoots;

public sealed class Tenant : AggregateRoot<Guid>
{
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }

    private Tenant() : base() { }

    private Tenant(string name, string? description) : base()
    {
        Name = name;
        Description = description;
        IsActive = true;
    }

    public static Tenant Create(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Tenant name cannot be null or empty.", nameof(name));

        return new Tenant(name.Trim(), description?.Trim());
    }

    public void Activate()
    {
        if (IsActive) return;
        IsActive = true;
        Touch();
    }

    public void Deactivate()
    {
        if (!IsActive) return;
        IsActive = false;
        Touch();
    }
}
