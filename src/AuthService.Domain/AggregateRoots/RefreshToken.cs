using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using src.BuldingBlocks.Domain;

namespace AuthService.Domain.AggregateRoots;

public class RefreshToken : BaseEntity<Guid>
{
    public Guid UserId { get; private set; }
    public string Token { get; private set; } = null!;
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }

    private RefreshToken() : base() { }

    public static RefreshToken Create(Guid userId, string token, DateTime expiresAt)
    {
        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = token,
            ExpiresAt = expiresAt
        };
    }

    public static RefreshToken Create(Guid userId, string token, DateTime expiresAt, DateTime? revokedAt)
    {
        return new RefreshToken
        {
            UserId = userId,
            Token = token,
            ExpiresAt = expiresAt,
            RevokedAt = revokedAt
        };
    }

    public void Revoke()
    {
        RevokedAt = DateTime.UtcNow;
        Touch();
    }
}
