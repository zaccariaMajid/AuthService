using AuthService.Domain.AggregateRoots;

namespace AuthService.Application.Interfaces.Repositories;

public interface IProductRepository : IEfRepository<Product>
{
    Task<Product?> GetByNameAsync(string name);
}
