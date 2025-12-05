using AuthService.Domain.Common;
using AuthService.Domain.Exceptions;
using src.BuldingBlocks.Domain;

namespace AuthService.Domain.AggregateRoots;

public sealed class Tenant : AggregateRoot<Guid>
{
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; }
    public ICollection<Product> Products { get; private set; } = new List<Product>();

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

    public void AddProduct(Product product)
    {
        if (product is null)
            throw new DomainException("Product cannot be null.", nameof(product));

        if (Products.Any(p => p.Id == product.Id))
            return;

        Products.Add(product);
        Touch();
    }
}
