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
    User correctUser = User.Create(
            Email.Create("newuser@example.com"),
            PasswordHash.Create("hash", "salt"),
            "Mario",
            "Rossi");
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<IPasswordHasher> _hasher = new();

    [TestMethod]
    public async Task Handle_ShouldCreateUser_WhenEmailNotExists()
    {
        // Arrange
        var command = new RegisterUserCommand(
            "Mario",
            "Rossi",
            "Password123!",
            "newuser@example.com"
        );

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
            "Mario",
            "Rossi",
            "Password123!",
            "newuser@example.com"
        );

        _userRepo.Setup(r => r.GetByEmailAsync(command.Email))
            .ReturnsAsync(correctUser);

        var handler = new RegisterUserCommandHandler(
            _userRepo.Object, _hasher.Object);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("User.EmailTaken", result.Error.Code);
        _userRepo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("  ")]
    public async Task Handle_ShouldFail_WhenPasswordIsEmpty(string password)
    {
        var command = new RegisterUserCommand(
            "Mario",
            "Rossi",
            password,
            "newuser@example.com"
        );

        _userRepo.Setup(r => r.GetByEmailAsync(command.Email))
            .ReturnsAsync(correctUser);

        var handler = new RegisterUserCommandHandler(
            _userRepo.Object, _hasher.Object);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("User.PasswordEmpty", result.Error.Code);
        _userRepo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
    }
}
