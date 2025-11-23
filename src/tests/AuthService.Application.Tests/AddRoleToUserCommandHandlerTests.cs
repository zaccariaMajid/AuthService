using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Application.Users.AddRoleToUser;
using AuthService.Domain.AggregateRoots;
using AuthService.Domain.ValueObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AuthService.Application.Tests;

[TestClass]
public class AddRoleToUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<IRoleRepository> _roleRepo = new();

    [TestMethod]
    public async Task Handle_ValidUserAndRole_ShouldReturnSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var user = User.Create(
            Email.Create("user@example.com"),
            PasswordHash.Create("hash", "salt"),
            "Mario",
            "Rossi");

        var role = Role.Create("Admin", "Administrator role");

        _userRepo.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(user);

        _roleRepo.Setup(r => r.GetByIdAsync(roleId))
            .ReturnsAsync(role);

        var command = new AddRoleToUserCommand(userId, roleId);

        var handler = new AddRoleToUserCommandHandler(
            _roleRepo.Object,
            _userRepo.Object);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsSuccess);

        // Verify the user contains the role
        Assert.IsTrue(user.Roles.Any(r => r.Name == "Admin"));

        // Verify the user was updated
        _userRepo.Verify(r => r.UpdateAsync(user), Times.Once);
    }

    [TestMethod]
    public async Task Handle_UserNotFound_ShouldReturnFailure()
    {
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        _userRepo.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        var command = new AddRoleToUserCommand(userId, roleId);

        var handler = new AddRoleToUserCommandHandler(
            _roleRepo.Object,
            _userRepo.Object);

        var result = await handler.HandleAsync(command, CancellationToken.None);

        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("User.NotFound", result.Error.Code);

        _userRepo.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    [TestMethod]
    public async Task Handle_RoleNotFound_ShouldReturnFailure()
    {
        var userId = Guid.NewGuid();
        var roleId = Guid.NewGuid();

        var user = User.Create(
            Email.Create("user@example.com"),
            PasswordHash.Create("hash", "salt"),
            "Mario",
            "Rossi");

        _userRepo.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(user);

        _roleRepo.Setup(r => r.GetByIdAsync(roleId))
            .ReturnsAsync((Role?)null);

        var command = new AddRoleToUserCommand(userId, roleId);

        var handler = new AddRoleToUserCommandHandler(
            _roleRepo.Object,
            _userRepo.Object);

        var result = await handler.HandleAsync(command, CancellationToken.None);

        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("Role.NotFound", result.Error.Code);

        _userRepo.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

}
