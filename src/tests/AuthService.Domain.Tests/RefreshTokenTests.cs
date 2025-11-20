using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Domain.AggregateRoots;
using AuthService.Domain.Exceptions;
using AuthService.Domain.ValueObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AuthService.Domain.Tests;

[TestClass]
public class RefreshTokenTests
{
    User correctUser = User.Create(
            Email.Create("user@example.com"),
            PasswordHash.Create("hash", "salt"),
            "Mario",
            "Rossi");
    [TestMethod]
    public void CreateRefreshToken_ShouldSetDataAndRaiseEvent()
    {
        var refreshToken = RefreshToken.Create(correctUser.Id, "sample_token_value", DateTime.UtcNow.AddDays(7));
        Assert.AreEqual("sample_token_value", refreshToken.Token);
        Assert.IsNull(refreshToken.RevokedAt);

        Assert.AreEqual(1, refreshToken.DomainEvents.Count);
    }

    [TestMethod]
    [DataRow("00000000-0000-0000-0000-000000000000", "sample_token_value", "2024-12-31")]
    [ExpectedException(typeof(DomainException))]
    public void CreateRefreshToken_InvalidUserIdOrToken_ShouldThrow(string userIdStr, string token, string expiresAtStr)
    {
        var userId = Guid.Parse(userIdStr);
        var expiresAt = DateTime.Parse(expiresAtStr);
        RefreshToken.Create(userId, token, expiresAt);
    }

    [TestMethod]
    public void Revoke_ShouldUpdateAndRaiseEvennt()
    {
        var refreshToken = RefreshToken.Create(correctUser.Id, "sample_token_value", DateTime.UtcNow.AddDays(7));
        Assert.AreEqual("sample_token_value", refreshToken.Token);
        Assert.IsNull(refreshToken.RevokedAt);

        refreshToken.Revoke();
        Assert.AreEqual(1, refreshToken.DomainEvents.Count);
    }
}
