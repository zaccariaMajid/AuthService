using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Application.Permissions.CreatePermission;
using AuthService.Domain.AggregateRoots;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AuthService.Application.Tests;

[TestClass]
public class CreatePermissionCommandHandlerTests
{
    private readonly Mock<IPermissionRepository> _permissionRepo = new();

    [TestMethod]
    public async Task Handle_ValidPermissionName_ShouldReturnSuccess()
    {
        // Arrange
        var command = new CreatePermissionCommand("ViewUsers", "Allows viewing user list");

        _permissionRepo.Setup(r => r.GetByNameAsync(command.Name))
            .ReturnsAsync((Permission?)null);

        var handler = new CreatePermissionCommandHandler(_permissionRepo.Object);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("ViewUsers", result.Value.Name);

        _permissionRepo.Verify(r => r.AddAsync(It.IsAny<Permission>()), Times.Once);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("  ")]
    public async Task Handle_EmptyPermissionName_ShouldReturnFailure(string permissionName)
    {
        // Arrange
        var command = new CreatePermissionCommand(permissionName, "Some description");

        var handler = new CreatePermissionCommandHandler(_permissionRepo.Object);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("Permission.InvalidName", result.Error.Code);

        _permissionRepo.Verify(r => r.AddAsync(It.IsAny<Permission>()), Times.Never);
    }

    [TestMethod]
    public async Task Handle_PermissionAlreadyExists_ShouldReturnFailure()
    {
        // Arrange
        var command = new CreatePermissionCommand("ViewUsers", "Allows viewing user list");

        var existing = Permission.Create("ViewUsers", "Allows viewing user list");

        _permissionRepo.Setup(r => r.GetByNameAsync(command.Name))
            .ReturnsAsync(existing);

        var handler = new CreatePermissionCommandHandler(_permissionRepo.Object);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("Permission.Exists", result.Error.Code);

        _permissionRepo.Verify(r => r.AddAsync(It.IsAny<Permission>()), Times.Never);
    }
}
