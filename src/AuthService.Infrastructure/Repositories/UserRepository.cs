using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Domain.AggregateRoots;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Repositories;

public class UserRepository : EfRepository<User>, IUserRepository
{
    public UserRepository(AuthDbContext db) : base(db)
    {
    }

    public async Task<User?> GetByEmailAsync(string email, Guid tenantId)
        => await _db.Users
            .FirstOrDefaultAsync(u => u.Email.Value == email && u.TenantId == tenantId);

}
