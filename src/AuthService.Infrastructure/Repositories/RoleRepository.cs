using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Domain.AggregateRoots;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Repositories;

public class RoleRepository : EfRepository<Role>, IRoleRepository
{
    public RoleRepository(AuthDbContext db) : base(db)
    {
    }

    public async Task<Role?> GetByNameAsync(string name, Guid tenantId)
        => await _db.Roles.FirstOrDefaultAsync(u => u.Name == name && u.TenantId == tenantId);

}
