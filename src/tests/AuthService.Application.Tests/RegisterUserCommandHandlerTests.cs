using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Application.Users.RegisterUser;
using AuthService.Domain.AggregateRoots;
using AuthService.Domain.Interfaces;
using AuthService.Domain.ValueObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AuthService.Application.Tests;

[TestClass]
public class RegisterUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<IPasswordHasher> _hasher = new();

    [TestMethod]
    public async Task Handle_ShouldCreateUser_WhenEmailNotExists()
    {
        // Arrange
        var command = new RegisterUserCommand(
            "newuser@example.com",
            "Password123!",
            "Mario",
            "Rossi");

        _userRepo.Setup(r => r.GetByEmailAsync(command.Email))
            .ReturnsAsync((User?)null);

        _hasher.Setup(h => h.GenerateSalt()).Returns("salt");
        _hasher.Setup(h => h.Hash(command.Password, "salt")).Returns("hash");

        var handler = new RegisterUserCommandHandler(
            _userRepo.Object, _hasher.Object);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual("newuser@example.com", result.Value.Email);
        _userRepo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
    }

    [TestMethod]
    public async Task Handle_ShouldFail_WhenEmailAlreadyExists()
    {
        var command = new RegisterUserCommand(
            "existing@example.com",
            "Password123!",
            "Mario",
            "Rossi");

        var existing = User.Create(
            Email.Create(command.Email),
            PasswordHash.Create("hash", "salt"),
            "Test",
            "User");

        _userRepo.Setup(r => r.GetByEmailAsync(command.Email))
            .ReturnsAsync(existing);

        var handler = new RegisterUserCommandHandler(
            _userRepo.Object, _hasher.Object);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("User.EmailTaken", result.Error.Code);
        _userRepo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
    }
}
