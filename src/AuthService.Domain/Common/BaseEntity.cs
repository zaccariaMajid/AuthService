using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Domain.Common;

namespace src.BuldingBlocks.Domain;

public abstract class BaseEntity<TId>
{
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public TId Id { get; protected set; } = default!;
    public DateTime CreatedAt { get; protected set; }
    public DateTime UpdatedAt { get; protected set; }

    protected BaseEntity()
    {
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddDomainEvent(IDomainEvent domainEvent)
        => _domainEvents.Add(domainEvent);
    public void RemoveDomainEvent(IDomainEvent domainEvent)
        => _domainEvents.Remove(domainEvent);
    public void ClearDomainEvents()
        => _domainEvents.Clear();
    public void Touch()
        => UpdatedAt = DateTime.UtcNow;
}
