using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Application.Roles.CreateRole;
using AuthService.Domain.AggregateRoots;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AuthService.Application.Tests;

[TestClass]
public class CreateRoleCommandHandlerTests
{
    private readonly Mock<IRoleRepository> _roleRepo = new();

    [TestMethod]
    public async Task Handle_ValidRoleName_ShouldReturnSuccess()
    {
        // Arrange
        var command = new CreateRoleCommand("Admin", "Administrator role");

        _roleRepo.Setup(r => r.GetByNameAsync(command.Name))
            .ReturnsAsync((Role?)null);

        var handler = new CreateRoleCommandHandler(_roleRepo.Object);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("Admin", result.Value.Name);

        _roleRepo.Verify(r => r.AddAsync(It.IsAny<Role>()), Times.Once);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("  ")]
    public async Task Handle_EmptyRoleName_ShouldReturnFailure(string roleName)
    {
        // Arrange
        var command = new CreateRoleCommand(roleName, "Some description");

        var handler = new CreateRoleCommandHandler(_roleRepo.Object);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("Role.InvalidName", result.Error.Code);

        _roleRepo.Verify(r => r.AddAsync(It.IsAny<Role>()), Times.Never);
    }

    [TestMethod]
    public async Task Handle_RoleAlreadyExists_ShouldReturnFailure()
    {
        // Arrange
        var command = new CreateRoleCommand("Admin", "Administrator role");

        var existingRole = Role.Create("Admin", "Administrator role");

        _roleRepo.Setup(r => r.GetByNameAsync(command.Name))
            .ReturnsAsync(existingRole);

        var handler = new CreateRoleCommandHandler(_roleRepo.Object);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("Role.Exists", result.Error.Code);

        _roleRepo.Verify(r => r.AddAsync(It.IsAny<Role>()), Times.Never);
    }
}
