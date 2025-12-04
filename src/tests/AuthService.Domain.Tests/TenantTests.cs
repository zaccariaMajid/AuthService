using System;
using AuthService.Domain.AggregateRoots;
using AuthService.Domain.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AuthService.Domain.Tests;

[TestClass]
public class TenantTests
{
    [TestMethod]
    public void CreateTenant_ShouldSetPropertiesAndBeActive()
    {
        var tenant = Tenant.Create("Acme", "Test tenant");

        Assert.AreEqual("Acme", tenant.Name);
        Assert.AreEqual("Test tenant", tenant.Description);
        Assert.IsTrue(tenant.IsActive);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("   ")]
    [ExpectedException(typeof(DomainException))]
    public void CreateTenant_InvalidName_ShouldThrow(string name)
    {
        Tenant.Create(name, "desc");
    }

    [TestMethod]
    public void DeactivateAndActivate_ShouldToggleStatus()
    {
        var tenant = Tenant.Create("Contoso", "Desc");

        tenant.Deactivate();
        Assert.IsFalse(tenant.IsActive);

        tenant.Activate();
        Assert.IsTrue(tenant.IsActive);
    }
}
