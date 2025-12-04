using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Domain.AggregateRoots;

namespace AuthService.Application.Interfaces.Repositories;

public interface IRoleRepository : IEfRepository<Role>
{
    Task<Role?> GetByNameAsync(string name, Guid tenantId);
}
