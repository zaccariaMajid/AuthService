using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Domain.Common;

namespace src.BuldingBlocks.Domain;

public abstract class BaseEntity<TId>
{
    private readonly List<IDomainEvents> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvents> DomainEvents => _domainEvents.AsReadOnly();

    public TId Id { get; protected set; } = default!;
    public DateTime CreatedAt { get; protected set; }
    public DateTime UpdatedAt { get; protected set; }

    protected BaseEntity()
    {
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    protected void AddDomainEvent(IDomainEvents domainEvent)
        => _domainEvents.Add(domainEvent);
    protected void RemoveDomainEvent(IDomainEvents domainEvent)
        => _domainEvents.Remove(domainEvent);
    protected void ClearDomainEvents()
        => _domainEvents.Clear();
    protected void Touch()
        => UpdatedAt = DateTime.UtcNow;
}
