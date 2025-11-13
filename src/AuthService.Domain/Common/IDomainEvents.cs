using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.Domain.Common;

public interface IDomainEvents
{
    DateTime OccurredOn { get; }
}
