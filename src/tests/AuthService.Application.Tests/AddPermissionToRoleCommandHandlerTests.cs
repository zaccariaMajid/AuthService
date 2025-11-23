using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Application.Roles.AddPermissionToRole;
using AuthService.Domain.AggregateRoots;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AuthService.Application.Tests;

[TestClass]
public class AddPermissionToRoleCommandHandlerTests
{
    private readonly Mock<IRoleRepository> _roleRepo = new();
    private readonly Mock<IPermissionRepository> _permissionRepo = new();

    [TestMethod]
    public async Task Handle_ValidRoleAndPermission_ShouldReturnSuccess()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var permissionId = Guid.NewGuid();

        var role = Role.Create("Manager", "Manager role");
        var permission = Permission.Create("EditUsers", "Allows editing users");

        _roleRepo.Setup(r => r.GetByIdAsync(roleId))
            .ReturnsAsync(role);

        _permissionRepo.Setup(p => p.GetByIdAsync(permissionId))
            .ReturnsAsync(permission);

        var command = new AddPermissionToRoleCommand(roleId, permissionId);

        var handler = new AddPermissionToRoleCommandHandler(
            _roleRepo.Object,
            _permissionRepo.Object);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsSuccess);

        // Verify permission added
        Assert.IsNotNull(role.Permissions);
        Assert.IsTrue(role.Permissions.Any(p => p.Name == "EditUsers"));

        _roleRepo.Verify(r => r.UpdateAsync(role), Times.Once);
    }

    [TestMethod]
    public async Task Handle_RoleNotFound_ShouldReturnFailure()
    {
        var roleId = Guid.NewGuid();
        var permissionId = Guid.NewGuid();

        _roleRepo.Setup(r => r.GetByIdAsync(roleId))
            .ReturnsAsync((Role?)null);

        var command = new AddPermissionToRoleCommand(roleId, permissionId);

        var handler = new AddPermissionToRoleCommandHandler(
            _roleRepo.Object,
            _permissionRepo.Object);

        var result = await handler.HandleAsync(command, CancellationToken.None);

        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("Role.NotFound", result.Error.Code);

        _roleRepo.Verify(r => r.UpdateAsync(It.IsAny<Role>()), Times.Never);
    }

    [TestMethod]
        public async Task Handle_PermissionNotFound_ShouldReturnFailure()
        {
            var roleId = Guid.NewGuid();
            var permissionId = Guid.NewGuid();

            var role = Role.Create("Manager", "Manager role");

            _roleRepo.Setup(r => r.GetByIdAsync(roleId))
                .ReturnsAsync(role);

            _permissionRepo.Setup(p => p.GetByIdAsync(permissionId))
                .ReturnsAsync((Permission?)null);

            var command = new AddPermissionToRoleCommand(roleId, permissionId);

            var handler = new AddPermissionToRoleCommandHandler(
                _roleRepo.Object,
                _permissionRepo.Object);

            var result = await handler.HandleAsync(command, CancellationToken.None);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual("Permission.NotFound", result.Error.Code);

            _roleRepo.Verify(r => r.UpdateAsync(It.IsAny<Role>()), Times.Never);
        }
}
