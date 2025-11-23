using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Application.Users.UserLogin;
using AuthService.Domain.AggregateRoots;
using AuthService.Domain.Interfaces;
using AuthService.Domain.ValueObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AuthService.Application.Tests;

[TestClass]
public class UserLoginCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<IPasswordHasher> _hasher = new();
    private readonly Mock<ITokenService> _tokenService = new();

    [TestMethod]
    public async Task Handle_ValidCredentials_ShouldReturnSuccess()
    {
        // Arrange
        var command = new UserLoginCommand("user@example.com", "Password123!");

        var passwordHash = "hash";
        var passwordSalt = "salt";

        var user = User.Create(
            Email.Create(command.Email),
            PasswordHash.Create(passwordHash, passwordSalt),
            "Mario",
            "Rossi");

        _userRepo.Setup(r => r.GetByEmailAsync(command.Email))
            .ReturnsAsync(user);

        _hasher.Setup(h => h.VerifyPassword(It.IsAny<string>(), command.Password, It.IsAny<string>()))
            .Returns(true);

        _tokenService.Setup(t => t.GenerateAccessToken(user))
            .Returns("access-token");

        _tokenService.Setup(t => t.GenerateRefreshToken(user.Id))
            .Returns(RefreshToken.Create(user.Id, "refresh-token", DateTime.UtcNow.AddDays(7)));

        var handler = new UserLoginCommandHandler(
            _userRepo.Object, _hasher.Object, _tokenService.Object);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(user.Id, result.Value.UserId);
        Assert.AreEqual("access-token", result.Value.AccessToken);
    }

    [TestMethod]
    public async Task Handle_UserNotFound_ShouldReturnFailure()
    {
        var command = new UserLoginCommand("missing@example.com", "Password123!");

        _userRepo.Setup(r => r.GetByEmailAsync(command.Email))
            .ReturnsAsync((User?)null);

        var handler = new UserLoginCommandHandler(
            _userRepo.Object, _hasher.Object, _tokenService.Object);

        var result = await handler.HandleAsync(command, CancellationToken.None);

        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("User.NotFound", result.Error.Code);
    }

    [TestMethod]
    [DataRow("", "Password123!")]
    [DataRow("   ", "Password123!")]
    public async Task Handle_EmptyEmail_ShouldReturnFailure(string email, string password)
    {
        var command = new UserLoginCommand(email, password);

        var handler = new UserLoginCommandHandler(
            _userRepo.Object, _hasher.Object, _tokenService.Object);

        var result = await handler.HandleAsync(command, CancellationToken.None);

        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("User.InvalidEmail", result.Error.Code);
    }

    [TestMethod]
    [DataRow("missing@example.com", "")]
    [DataRow("missing@example.com", "  ")]
    public async Task Handle_EmptyPassword_ShouldReturnFailure(string email, string password)
    {
        var command = new UserLoginCommand(email, password);

        var handler = new UserLoginCommandHandler(
            _userRepo.Object, _hasher.Object, _tokenService.Object);

        var result = await handler.HandleAsync(command, CancellationToken.None);

        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("User.InvalidPassword", result.Error.Code);
    }

    [TestMethod]
    public async Task Handle_InvalidPassword_ShouldReturnFailure()
    {
        var command = new UserLoginCommand("user@example.com", "wrong");

        var user = User.Create(
            Email.Create(command.Email),
            PasswordHash.Create("hash", "salt"),
            "Mario",
            "Rossi");

        _userRepo.Setup(r => r.GetByEmailAsync(command.Email))
            .ReturnsAsync(user);

        _hasher.Setup(h => h.VerifyPassword(It.IsAny<string>(), command.Password, It.IsAny<string>()))
            .Returns(false);

        var handler = new UserLoginCommandHandler(
            _userRepo.Object, _hasher.Object, _tokenService.Object);

        var result = await handler.HandleAsync(command, CancellationToken.None);

        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("User.InvalidCredentials", result.Error.Code);
    }
}
