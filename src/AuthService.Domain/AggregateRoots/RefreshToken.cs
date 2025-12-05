using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Domain.Common;
using AuthService.Domain.Events;
using AuthService.Domain.Exceptions;
using src.BuldingBlocks.Domain;

namespace AuthService.Domain.AggregateRoots;

public class RefreshToken : AggregateRoot<Guid>
{
    public Guid UserId { get; private set; }
    public string Token { get; private set; } = null!;
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }

    private RefreshToken() : base() { }
    private RefreshToken(Guid userId, string token, DateTime expiresAt, DateTime? revokedAt = null) : base()
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
        RevokedAt = revokedAt;

        AddDomainEvent(new RefreshTokenCreated(UserId));
    }
    public static RefreshToken Create(Guid userId, string token, DateTime expiresAt)
    {
        if (userId == Guid.Empty)
            throw new DomainException("UserId cannot be empty.", nameof(userId));

        if (string.IsNullOrWhiteSpace(token))
            throw new DomainException("Token cannot be null or empty.", nameof(token));

        return new RefreshToken
        (
            userId,
            token,
            expiresAt
        );
    }

    public void Revoke()
    {
        RevokedAt = DateTime.UtcNow;
        Touch();
    }
}
