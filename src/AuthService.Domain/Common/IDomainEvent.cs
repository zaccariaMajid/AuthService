using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.Domain.Common;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
