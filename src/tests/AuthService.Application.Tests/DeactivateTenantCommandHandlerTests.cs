using System;
using System.Threading.Tasks;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Application.Tenants.DeactivateTenant;
using AuthService.Domain.AggregateRoots;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AuthService.Application.Tests;

[TestClass]
public class DeactivateTenantCommandHandlerTests
{
    private readonly Mock<ITenantRepository> _tenants = new();

    [TestMethod]
    public async Task Handle_TenantExists_ShouldDeactivate()
    {
        var tenantId = Guid.NewGuid();
        var tenant = Tenant.Create("Acme");

        _tenants.Setup(t => t.GetByIdAsync(tenantId))
            .ReturnsAsync(tenant);

        var handler = new DeactivateTenantCommandHandler(_tenants.Object);

        var result = await handler.HandleAsync(new DeactivateTenantCommand(tenantId), CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsFalse(tenant.IsActive);
        _tenants.Verify(t => t.UpdateAsync(tenant), Times.Once);
    }

    [TestMethod]
    public async Task Handle_TenantMissing_ShouldReturnFailure()
    {
        var tenantId = Guid.NewGuid();
        _tenants.Setup(t => t.GetByIdAsync(tenantId))
            .ReturnsAsync((Tenant?)null);

        var handler = new DeactivateTenantCommandHandler(_tenants.Object);

        var result = await handler.HandleAsync(new DeactivateTenantCommand(tenantId), CancellationToken.None);

        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("Tenant.NotFound", result.Error.Code);
        _tenants.Verify(t => t.UpdateAsync(It.IsAny<Tenant>()), Times.Never);
    }
}
