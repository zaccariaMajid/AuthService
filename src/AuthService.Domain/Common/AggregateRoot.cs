using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using src.BuldingBlocks.Domain;

namespace AuthService.Domain.Common;

public abstract class AggregateRoot<TId> : BaseEntity<TId>
{
    protected AggregateRoot() : base() { }
}
