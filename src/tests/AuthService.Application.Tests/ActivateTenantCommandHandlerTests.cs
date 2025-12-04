using System;
using System.Threading.Tasks;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Application.Tenants.ActivateTenant;
using AuthService.Domain.AggregateRoots;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AuthService.Application.Tests;

[TestClass]
public class ActivateTenantCommandHandlerTests
{
    private readonly Mock<ITenantRepository> _tenants = new();

    [TestMethod]
    public async Task Handle_TenantExists_ShouldActivate()
    {
        var tenantId = Guid.NewGuid();
        var tenant = Tenant.Create("Acme");
        tenant.Deactivate();

        _tenants.Setup(t => t.GetByIdAsync(tenantId))
            .ReturnsAsync(tenant);

        var handler = new ActivateTenantCommandHandler(_tenants.Object);

        var result = await handler.HandleAsync(new ActivateTenantCommand(tenantId), CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsTrue(tenant.IsActive);
        _tenants.Verify(t => t.UpdateAsync(tenant), Times.Once);
    }

    [TestMethod]
    public async Task Handle_TenantMissing_ShouldReturnFailure()
    {
        var tenantId = Guid.NewGuid();
        _tenants.Setup(t => t.GetByIdAsync(tenantId))
            .ReturnsAsync((Tenant?)null);

        var handler = new ActivateTenantCommandHandler(_tenants.Object);

        var result = await handler.HandleAsync(new ActivateTenantCommand(tenantId), CancellationToken.None);

        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("Tenant.NotFound", result.Error.Code);
        _tenants.Verify(t => t.UpdateAsync(It.IsAny<Tenant>()), Times.Never);
    }
}
