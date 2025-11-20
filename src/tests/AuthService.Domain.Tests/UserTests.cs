using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Domain.AggregateRoots;
using AuthService.Domain.ValueObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AuthService.Domain.Tests;

[TestClass]
public class UserTests
{
    [TestMethod]
    public void CreateUser_ShouldSetDataAndRaiseEvent()
    {
        var email = Email.Create("user@example.com");
        var password = PasswordHash.Create("hash", "salt");

        var user = User.Create(email, password, "Mario", "Rossi");

        Assert.AreEqual(email, user.Email);
        Assert.AreEqual("Mario", user.FirstName);
        Assert.AreEqual("Rossi", user.LastName);
        Assert.IsFalse(user.IsActive);

        Assert.AreEqual(1, user.DomainEvents.Count);
    }

    [TestMethod]
    public void Activate_UserInactive_ShouldActivateAndRaiseEvent()
    {
        var user = User.Create(
            Email.Create("user@example.com"),
            PasswordHash.Create("hash", "salt"),
            "Mario",
            "Rossi");

        user.ClearDomainEvents();

        user.Activate();

        Assert.IsTrue(user.IsActive);
        Assert.AreEqual(1, user.DomainEvents.Count);
    }

    [TestMethod]
    public void ChangePassword_ShouldSetNewHashAndRaiseEvent()
    {
        var user = User.Create(
            Email.Create("user@example.com"),
            PasswordHash.Create("hash", "salt"),
            "Mario",
            "Rossi");

        user.ClearDomainEvents();

        var newHash = PasswordHash.Create("newHash", "newSalt");

        user.ChangePassword(newHash);

        Assert.AreEqual("newHash", user.PasswordHash.Hash);
        Assert.AreEqual(1, user.DomainEvents.Count);
    }
}