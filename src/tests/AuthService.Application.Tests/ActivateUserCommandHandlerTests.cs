using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Application.Users.ActivateUser;
using AuthService.Domain.AggregateRoots;
using AuthService.Domain.ValueObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AuthService.Application.Tests;

[TestClass]
public class ActivateUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepo = new();

    [TestMethod]
    public async Task Handle_ValidInactiveUser_ShouldReturnSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();

        var user = User.Create(
            Email.Create("user@example.com"),
            PasswordHash.Create("hash", "salt"),
            "Mario",
            "Rossi",
            tenantId);

        // Ensure inactive
        Assert.IsFalse(user.IsActive);

        _userRepo.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(user);

        var command = new ActivateUserCommand(userId);

        var handler = new ActivateUserCommandHandler(_userRepo.Object);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsTrue(user.IsActive);

        _userRepo.Verify(r => r.UpdateAsync(user), Times.Once);
    }

    [TestMethod]
    public async Task Handle_UserNotFound_ShouldReturnFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _userRepo.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        var command = new ActivateUserCommand(userId);

        var handler = new ActivateUserCommandHandler(_userRepo.Object);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("User.NotFound", result.Error.Code);

        _userRepo.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    [TestMethod]
    public async Task Handle_UserAlreadyActive_ShouldReturnFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();

        var user = User.Create(
            Email.Create("user@example.com"),
            PasswordHash.Create("hash", "salt"),
            "Mario",
            "Rossi",
            tenantId);

        user.Activate(); // Make user active

        _userRepo.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(user);

        var command = new ActivateUserCommand(userId);

        var handler = new ActivateUserCommandHandler(_userRepo.Object);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("User.AlreadyActive", result.Error.Code);

        _userRepo.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }
}
