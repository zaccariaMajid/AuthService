using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Domain.Exceptions;
using AuthService.Domain.ValueObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AuthService.Domain.Tests;

[TestClass]
public class PasswordHashTests
{
    [TestMethod]
    public void PasswordHash_Creation_Succeeds_For_Valid_PasswordHash()
    {
        // Arrange
        string value = "passwords";
        string salt = "somesalt";

        // Act
        var passwordHash = PasswordHash.Create(value, salt);

        // Assert
        Assert.IsNotNull(passwordHash);
    }

    [DataTestMethod]
    [DataRow("", "somesalt")]
    [DataRow(" ", "somesalt")]
    [DataRow("pwsd", "")]
    [ExpectedException(typeof(DomainException))]
    public void Create_InvalidPasswordHash_ShouldThrow(string pw, string salt)
    {
        PasswordHash.Create(pw, salt);
    }
}
