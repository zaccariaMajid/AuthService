using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Domain.AggregateRoots;
using AuthService.Domain.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AuthService.Domain.Tests;

[TestClass]
public class RoleTests
{
    private readonly Guid _tenantId = Guid.NewGuid();
    Role correctRole;
    Permission permission = Permission.Create("Read", "Read permission");

    public RoleTests()
    {
        correctRole = Role.Create("Admin", "Administrator role", tenantId: _tenantId);
    }

    [TestMethod]
    public void CreateRole_ShouldSetDataAndRaiseEvent()
    {
        var role = Role.Create("Admin", "Administrator role", tenantId: _tenantId);

        Assert.AreEqual("Admin", role.Name);
        Assert.AreEqual("Administrator role", role.Description);

        Assert.AreEqual(1, role.DomainEvents.Count);
    }

    [TestMethod]
    [DataRow("", "Administrator role")]
    [ExpectedException(typeof(DomainException))]
    public void CreateRole_InvalidName_ShouldThrow(string name, string description)
    {
        Role.Create(name, description, tenantId: _tenantId);
    }

    [TestMethod]
    public void AddPermission_ShouldSetAndRaiseEvent()
    {
        // Arrange
        correctRole.ClearDomainEvents();

        // Act
        correctRole.AddPermission(permission);

        Assert.IsNotNull(correctRole.Permissions);
        Assert.IsTrue(correctRole.Permissions.Count > 0);
        Assert.AreEqual(1, correctRole.DomainEvents.Count);
    }

    [TestMethod]
    [ExpectedException(typeof(DomainException))]
    public void AddPermission_InvalidPermission_ShouldThrow()
    {
        // Arrange
        correctRole.ClearDomainEvents();

        // Act
        correctRole.AddPermission(null!);

        Assert.IsNotNull(correctRole.Permissions);
        Assert.IsTrue(correctRole.Permissions.Count > 0);
        Assert.AreEqual(1, correctRole.DomainEvents.Count);
    }

    [TestMethod]
    public void RemovePermission_ShouldSetAndRaiseEvent()
    {
        // Arrange
        correctRole.ClearDomainEvents();

        // Act
        correctRole.AddPermission(permission);
        correctRole.RemovePermission(permission);

        Assert.IsNull(correctRole.Permissions);
        Assert.AreEqual(2, correctRole.DomainEvents.Count);
    }
}
