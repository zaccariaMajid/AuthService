using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Infrastructure;

namespace AuthService.Application.Interfaces.Repositories;

public class EfRepository<T> where T : class
{
    protected readonly AuthDbContext _db;

    public EfRepository(AuthDbContext db)
    {
        _db = db;
    }

    public virtual async Task<T?> GetByIdAsync(Guid id)
        => await _db.Set<T>().FindAsync(id);

    public virtual async Task AddAsync(T entity)
    {
        _db.Set<T>().Add(entity);
        await _db.SaveChangesAsync();
    }

    public virtual async Task UpdateAsync(T entity)
    {
        _db.Set<T>().Update(entity);
        await _db.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(T entity)
    {
        _db.Set<T>().Remove(entity);
        await _db.SaveChangesAsync();
    }
}
