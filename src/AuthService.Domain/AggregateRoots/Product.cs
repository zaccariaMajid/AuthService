using AuthService.Domain.Common;
using AuthService.Domain.Exceptions;
using src.BuldingBlocks.Domain;

namespace AuthService.Domain.AggregateRoots;

public sealed class Product : AggregateRoot<Guid>
{
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }

    private Product() : base() { }

    private Product(string name, string? description) : base()
    {
        Name = name;
        Description = description;
        IsActive = true;
    }

    public static Product Create(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Product name cannot be null or empty.", nameof(name));

        return new Product(name.Trim(), description?.Trim());
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
