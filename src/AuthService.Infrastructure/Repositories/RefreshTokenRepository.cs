using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Domain.AggregateRoots;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Infrastructure.Repositories;

public class RefreshTokenRepository : EfRepository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(AuthDbContext db) : base(db)
    {
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
        => await _db.RefreshTokens.FirstOrDefaultAsync(u => u.Token == token);

}