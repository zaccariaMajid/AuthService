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
            "Rossi",
            Guid.NewGuid());
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<IPasswordHasher> _hasher = new();
    private readonly Mock<ITenantRepository> _tenants = new();

    [TestMethod]
    public async Task Handle_ShouldCreateUser_WhenEmailNotExists()
    {
        var tenantId = Guid.NewGuid();
        // Arrange
        var command = new RegisterUserCommand(
            "Mario",
            "Rossi",
            "Password123!",
            "newuser@example.com",
            tenantId
        );

        _tenants.Setup(t => t.GetByIdAsync(tenantId))
            .ReturnsAsync(Tenant.Create("Tenant A"));

        _userRepo.Setup(r => r.GetByEmailAsync(command.Email, tenantId))
            .ReturnsAsync((User?)null);

        _hasher.Setup(h => h.GenerateSalt()).Returns("salt");
        _hasher.Setup(h => h.Hash(command.Password, "salt")).Returns("hash");

        var handler = new RegisterUserCommandHandler(
            _userRepo.Object, _hasher.Object, _tenants.Object);

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
        var tenantId = Guid.NewGuid();
        var command = new RegisterUserCommand(
            "Mario",
            "Rossi",
            "Password123!",
            "newuser@example.com",
            tenantId
        );

        _tenants.Setup(t => t.GetByIdAsync(tenantId))
            .ReturnsAsync(Tenant.Create("Tenant A"));

        _userRepo.Setup(r => r.GetByEmailAsync(command.Email, tenantId))
            .ReturnsAsync(correctUser);

        var handler = new RegisterUserCommandHandler(
            _userRepo.Object, _hasher.Object, _tenants.Object);

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
        var tenantId = Guid.NewGuid();
        var command = new RegisterUserCommand(
            "Mario",
            "Rossi",
            password,
            "newuser@example.com",
            tenantId
        );

        _tenants.Setup(t => t.GetByIdAsync(tenantId))
            .ReturnsAsync(Tenant.Create("Tenant A"));

        _userRepo.Setup(r => r.GetByEmailAsync(command.Email, tenantId))
            .ReturnsAsync(correctUser);

        var handler = new RegisterUserCommandHandler(
            _userRepo.Object, _hasher.Object, _tenants.Object);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual("User.PasswordEmpty", result.Error.Code);
        _userRepo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
    }
}
