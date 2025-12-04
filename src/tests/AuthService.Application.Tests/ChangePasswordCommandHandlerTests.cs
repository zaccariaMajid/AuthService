using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Application.Users.ChangePassword;
using AuthService.Domain.AggregateRoots;
using AuthService.Domain.Interfaces;
using AuthService.Domain.ValueObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AuthService.Application.Tests;

[TestClass]
public class ChangePasswordCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<IPasswordHasher> _hasher = new();

    [TestMethod]
    public async Task Handle_ValidOldPassword_ShouldReturnSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();

        var user = User.Create(
            Email.Create("user@example.com"),
            PasswordHash.Create("oldHash", "oldSalt"),
            "Mario",
            "Rossi",
            tenantId);

        // mock GetByIdAsync
        _userRepo.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(user);

        // mock Verify(oldPassword)
        _hasher.Setup(h => h.VerifyPassword("oldHash", "OldPass123!", "oldSalt"))
            .Returns(true);

        // mock GenerateSalt + Hash(newPassword)
        _hasher.Setup(h => h.GenerateSalt()).Returns("newSalt");
        _hasher.Setup(h => h.Hash("NewPass456!", "newSalt"))
            .Returns("newHash");

        var command = new ChangePasswordCommand(
            userId,
            "OldPass123!",
            "NewPass456!"
        );

        var handler = new ChangePasswordCommandHandler(
            _userRepo.Object,
            _hasher.Object);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("newHash", user.PasswordHash.Hash);

        // User must be updated
        _userRepo.Verify(r => r.UpdateAsync(user), Times.Once);
    }

    [TestMethod]
    public async Task Handle_UserNotFound_ShouldReturnFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _userRepo.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        var command = new ChangePasswordCommand(
            userId,
            "anything",
            "newPassword"
        );

        var handler = new ChangePasswordCommandHandler(
            _userRepo.Object,
            _hasher.Object);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("Password.UserNotFound", result.Error.Code);

        _userRepo.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    [TestMethod]
    public async Task Handle_InvalidOldPassword_ShouldReturnFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var tenantId = Guid.NewGuid();

        var user = User.Create(
            Email.Create("user@example.com"),
            PasswordHash.Create("oldHash", "oldSalt"),
            "Mario",
            "Rossi",
            tenantId);

        _userRepo.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(user);

        _hasher.Setup(h => h.VerifyPassword("wrong", "oldHash", "oldSalt"))
            .Returns(false);

        var command = new ChangePasswordCommand(
            userId,
            "wrong",
            "newPassword"
        );

        var handler = new ChangePasswordCommandHandler(
            _userRepo.Object,
            _hasher.Object);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("Password.InvalidOldPassword", result.Error.Code);

        _userRepo.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    public async Task Handle_OldPasswordEmpty_ShouldReturnFailure(string oldPassword)
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new ChangePasswordCommand(
            userId,
            oldPassword,
            "newPassword"
        );

        var handler = new ChangePasswordCommandHandler(
            _userRepo.Object,
            _hasher.Object);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("Password.CurrentPasswordEmpty", result.Error.Code);

        _userRepo.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }


    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    public async Task Handle_NewPasswordEmpty_ShouldReturnFailure(string newPassword)
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new ChangePasswordCommand(
            userId,
            "wrong",
            newPassword
        );

        var handler = new ChangePasswordCommandHandler(
            _userRepo.Object,
            _hasher.Object);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("Password.NewPasswordEmpty", result.Error.Code);

        _userRepo.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

}
