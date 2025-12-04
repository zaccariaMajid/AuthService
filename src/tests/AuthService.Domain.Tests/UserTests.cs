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
public class UserTests
{
    private readonly Guid _tenantId = Guid.NewGuid();
    User correctUser;
    Role role;

    public UserTests()
    {
        correctUser = User.Create(
            Email.Create("user@example.com"),
            PasswordHash.Create("hash", "salt"),
            "Mario",
            "Rossi",
            _tenantId);
        role = Role.Create("Admin", "Administrator role", tenantId: _tenantId);
    }
    
    [TestMethod]
    public void CreateUser_ShouldSetDataAndRaiseEvent()
    {
        var email = Email.Create("user@example.com");
        var password = PasswordHash.Create("hash", "salt");

        var user = User.Create(email, password, "Mario", "Rossi", _tenantId);

        Assert.AreEqual(email, user.Email);
        Assert.AreEqual("Mario", user.FirstName);
        Assert.AreEqual("Rossi", user.LastName);
        Assert.IsFalse(user.IsActive);

        Assert.AreEqual(1, user.DomainEvents.Count);
    }

    [TestMethod]
    [DataRow("", "Rossi")]
    [DataRow("Mario", "")]
    [ExpectedException(typeof(DomainException))]
    public void CreateUser_InvalidName_ShouldThrow(string firstName, string lastName)
    {
        var email = Email.Create("user@example.com");
        var password = PasswordHash.Create("hash", "salt");

        User.Create(email, password, firstName, lastName, _tenantId);
    }

    [TestMethod]
    public void ChangePassword_ShouldSetNewHashAndRaiseEvent()
    {
        correctUser.ClearDomainEvents();

        var newHash = PasswordHash.Create("newHash", "newSalt");

        correctUser.ChangePassword(newHash);

        Assert.AreEqual("newHash", correctUser.PasswordHash.Hash);
        Assert.AreEqual(1, correctUser.DomainEvents.Count);
    }

    [TestMethod]
    public void Activate_ShouldSetNewHashAndRaiseEvent()
    {
        // Arrange
        correctUser.ClearDomainEvents();

        // Act
        correctUser.Activate();

        Assert.IsTrue(correctUser.IsActive);
        Assert.AreEqual(1, correctUser.DomainEvents.Count);
    }

    [TestMethod]
    public void Dectivate_ShouldSetNewHashAndRaiseEvent()
    {
        // Arrange
        correctUser.ClearDomainEvents();

        // Act
        correctUser.Activate();
        correctUser.Deactivate();

        Assert.IsFalse(correctUser.IsActive);
        Assert.AreEqual(2, correctUser.DomainEvents.Count);
    }

    [TestMethod]
    public void AssignRole_ShouldSetNewHashAndRaiseEvent()
    {
        // Arrange
        correctUser.ClearDomainEvents();

        // Act
        correctUser.AssignRole(role);

        Assert.IsTrue(correctUser.Roles.Count > 0);
        Assert.AreEqual(1, correctUser.DomainEvents.Count);
    }


    [TestMethod]
    public void RemoveRole_ShouldSetNewHashAndRaiseEvent()
    {
        // Arrange
        correctUser.ClearDomainEvents();

        // Act
        correctUser.AssignRole(role);
        correctUser.RemoveRole(role);

        Assert.IsTrue(correctUser.Roles.Count == 0);
        Assert.AreEqual(1, correctUser.DomainEvents.Count);
    }
}
