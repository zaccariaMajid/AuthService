using AuthService.Application.Interfaces.Repositories;
using AuthService.Domain.AggregateRoots;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Repositories;

public class ProductRepository : EfRepository<Product>, IProductRepository
{
    public ProductRepository(AuthDbContext db) : base(db)
    {
    }

    public async Task<Product?> GetByNameAsync(string name)
        => await _db.Set<Product>()
            .FirstOrDefaultAsync(p => p.Name == name);
}

