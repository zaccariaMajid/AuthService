using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Domain.AggregateRoots;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Repositories;

public class PermissionRepository : EfRepository<Permission>, IPermissionRepository
{
    public PermissionRepository(AuthDbContext db) : base(db)
    {
    }

    public async Task<Permission?> GetByNameAsync(string name)
        => await _db.Permissions.FirstOrDefaultAsync(u => u.Name == name);

}
