using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Domain.AggregateRoots;

namespace AuthService.Application.Interfaces.Repositories;

public interface IUserRepository : IEfRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
}
