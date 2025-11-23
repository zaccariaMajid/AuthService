using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Application.Users.RefreshToken;
using AuthService.Domain.AggregateRoots;
using AuthService.Domain.Interfaces;
using AuthService.Domain.ValueObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AuthService.Application.Tests;

[TestClass]
public class RefreshTokenCommandHandlerTests
{
    private readonly Mock<IRefreshTokenRepository> _tokenRepo = new();
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<ITokenService> _tokenService = new();

    [TestMethod]
    public async Task Handle_ValidRefreshToken_ShouldReturnSuccess()
    {
        // Arrange
        var refreshTokenString = "valid-refresh-token";

        var user = User.Create(
            Email.Create("user@example.com"),
            PasswordHash.Create("hash", "salt"),
            "Mario",
            "Rossi");

        var refreshToken = RefreshToken.Create(
            user.Id,
            refreshTokenString,
            DateTime.UtcNow.AddDays(7));

        _tokenRepo.Setup(r => r.GetByTokenAsync(refreshTokenString))
            .ReturnsAsync(refreshToken);

        _userRepo.Setup(r => r.GetByIdAsync(user.Id))
            .ReturnsAsync(user);

        _tokenService.Setup(t => t.GenerateAccessToken(user))
            .Returns("new-access-token");

        var newRefreshToken = RefreshToken.Create(
            user.Id,
            "new-refresh-token",
            DateTime.UtcNow.AddDays(7));

        _tokenService.Setup(t => t.GenerateRefreshToken(user.Id))
            .Returns(newRefreshToken);

        var command = new RefreshTokenCommand(refreshTokenString);

        var handler = new RefreshTokenCommandHandler(
            _tokenRepo.Object,
            _userRepo.Object,
            _tokenService.Object);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(user.Id, result.Value.UserId);
        Assert.AreEqual("new-access-token", result.Value.AccessToken);

        // _tokenRepo.Verify(r => r.AddAsync(It.IsAny<RefreshToken>()), Times.Once);
    }

    [TestMethod]
    public async Task Handle_TokenNotFound_ShouldReturnFailure()
    {
        var command = new RefreshTokenCommand("missing-token");

        _tokenRepo.Setup(r => r.GetByTokenAsync("missing-token"))
            .ReturnsAsync((RefreshToken?)null);

        var handler = new RefreshTokenCommandHandler(
            _tokenRepo.Object,
            _userRepo.Object,
            _tokenService.Object);

        var result = await handler.HandleAsync(command, CancellationToken.None);

        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("RefreshToken.InvalidRefreshToken", result.Error.Code);
    }

    [TestMethod]
    public async Task Handle_TokenExpired_ShouldReturnFailure()
    {
        var userId = Guid.NewGuid();
        var refreshToken = RefreshToken.Create(
            userId,
            "expired-token",
            DateTime.UtcNow.AddDays(-1)); // expired

        _tokenRepo.Setup(r => r.GetByTokenAsync("expired-token"))
            .ReturnsAsync(refreshToken);

        var command = new RefreshTokenCommand("expired-token");

        var handler = new RefreshTokenCommandHandler(
            _tokenRepo.Object,
            _userRepo.Object,
            _tokenService.Object);

        var result = await handler.HandleAsync(command, CancellationToken.None);

        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("RefreshToken.InvalidRefreshToken", result.Error.Code);
    }

    [TestMethod]
    public async Task Handle_TokenRevoked_ShouldReturnFailure()
    {
        var userId = Guid.NewGuid();
        var refreshToken = RefreshToken.Create(
            userId,
            "revoked-token",
            DateTime.UtcNow.AddDays(5));

        refreshToken.Revoke(); // domain behavior

        _tokenRepo.Setup(r => r.GetByTokenAsync("revoked-token"))
            .ReturnsAsync(refreshToken);

        var command = new RefreshTokenCommand("revoked-token");

        var handler = new RefreshTokenCommandHandler(
            _tokenRepo.Object,
            _userRepo.Object,
            _tokenService.Object);

        var result = await handler.HandleAsync(command, CancellationToken.None);

        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("RefreshToken.InvalidRefreshToken", result.Error.Code);
    }

    [TestMethod]
    public async Task Handle_UserNotFound_ShouldReturnFailure()
    {
        var userId = Guid.NewGuid();

        var refreshToken = RefreshToken.Create(
            userId,
            "valid-refresh-token",
            DateTime.UtcNow.AddDays(3));

        _tokenRepo.Setup(r => r.GetByTokenAsync("valid-refresh-token"))
            .ReturnsAsync(refreshToken);

        _userRepo.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        var command = new RefreshTokenCommand("valid-refresh-token");

        var handler = new RefreshTokenCommandHandler(
            _tokenRepo.Object,
            _userRepo.Object,
            _tokenService.Object);

        var result = await handler.HandleAsync(command, CancellationToken.None);

        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("RefreshToken.UserNotFound", result.Error.Code);
    }
}
