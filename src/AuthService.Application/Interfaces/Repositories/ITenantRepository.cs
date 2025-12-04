using AuthService.Domain.AggregateRoots;

namespace AuthService.Application.Interfaces.Repositories;

public interface ITenantRepository : IEfRepository<Tenant>
{
    Task<Tenant?> GetByNameAsync(string name);
}
