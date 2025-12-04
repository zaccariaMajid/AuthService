using AuthService.Application.Interfaces.Repositories;
using AuthService.Domain.AggregateRoots;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Repositories;

public class TenantRepository : EfRepository<Tenant>, ITenantRepository
{
    public TenantRepository(AuthDbContext db) : base(db)
    {
    }

    public async Task<Tenant?> GetByNameAsync(string name)
        => await _db.Set<Tenant>()
            .FirstOrDefaultAsync(t => t.Name == name);
}
