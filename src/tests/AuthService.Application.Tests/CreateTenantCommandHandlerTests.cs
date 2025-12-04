using System;
using System.Threading.Tasks;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Application.Tenants.CreateTenant;
using AuthService.Domain.AggregateRoots;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AuthService.Application.Tests;

[TestClass]
public class CreateTenantCommandHandlerTests
{
    private readonly Mock<ITenantRepository> _tenants = new();

    [TestMethod]
    public async Task Handle_ValidTenant_ShouldReturnSuccess()
    {
        var command = new CreateTenantCommand("Acme", "Desc");
        _tenants.Setup(t => t.GetByNameAsync("Acme"))
            .ReturnsAsync((Tenant?)null);

        var handler = new CreateTenantCommandHandler(_tenants.Object);

        var result = await handler.HandleAsync(command, CancellationToken.None);

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("Acme", result.Value.Name);
        _tenants.Verify(t => t.AddAsync(It.IsAny<Tenant>()), Times.Once);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("   ")]
    public async Task Handle_InvalidName_ShouldReturnFailure(string name)
    {
        var command = new CreateTenantCommand(name, "Desc");
        var handler = new CreateTenantCommandHandler(_tenants.Object);

        var result = await handler.HandleAsync(command, CancellationToken.None);

        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("Tenant.InvalidName", result.Error.Code);
        _tenants.Verify(t => t.AddAsync(It.IsAny<Tenant>()), Times.Never);
    }

    [TestMethod]
    public async Task Handle_TenantExists_ShouldReturnFailure()
    {
        var command = new CreateTenantCommand("Acme", "Desc");
        _tenants.Setup(t => t.GetByNameAsync("Acme"))
            .ReturnsAsync(Tenant.Create("Acme"));

        var handler = new CreateTenantCommandHandler(_tenants.Object);

        var result = await handler.HandleAsync(command, CancellationToken.None);

        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("Tenant.Exists", result.Error.Code);
        _tenants.Verify(t => t.AddAsync(It.IsAny<Tenant>()), Times.Never);
    }
}
