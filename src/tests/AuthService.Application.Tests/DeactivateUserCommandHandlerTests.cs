using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Application.Users.DeactivateUser;
using AuthService.Domain.AggregateRoots;
using AuthService.Domain.ValueObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AuthService.Application.Tests;

[TestClass]
public class DeactivateUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepo = new();

    [TestMethod]
    public async Task Handle_ValidActiveUser_ShouldReturnSuccess()
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

        // Make the user active
        user.Activate();
        Assert.IsTrue(user.IsActive);

        _userRepo.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(user);

        var command = new DeactivateUserCommand(userId);

        var handler = new DeactivateUserCommandHandler(_userRepo.Object);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.IsFalse(user.IsActive);

        _userRepo.Verify(r => r.UpdateAsync(user), Times.Once);
    }

    [TestMethod]
    public async Task Handle_UserNotFound_ShouldReturnFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _userRepo.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        var command = new DeactivateUserCommand(userId);

        var handler = new DeactivateUserCommandHandler(_userRepo.Object);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("User.NotFound", result.Error.Code);

        _userRepo.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    [TestMethod]
    public async Task Handle_UserAlreadyInactive_ShouldReturnFailure()
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

        // Ensure user is inactive
        Assert.IsFalse(user.IsActive);

        _userRepo.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(user);

        var command = new DeactivateUserCommand(userId);

        var handler = new DeactivateUserCommandHandler(_userRepo.Object);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("User.AlreadyInactive", result.Error.Code);

        _userRepo.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }
}
