using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.Application.Interfaces.Repositories;

public interface IEfRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);
    Task AddAsync(T Entity);
    Task UpdateAsync(T Entity);
    Task DeleteAsync(T Entity);
}
