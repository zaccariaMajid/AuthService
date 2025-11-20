using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Domain.Exceptions;
using AuthService.Domain.ValueObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AuthService.Domain.Tests;

[TestClass]
public class Emailtests
{
    [TestMethod]
    public void Email_Creation_Succeeds_For_Valid_Email()
    {
        // Arrange
        string value = "mail@gmail.com";

        // Act
        var email = Email.Create(value);

        // Assert
        Assert.IsNotNull(email);
        Assert.AreEqual(value.ToLowerInvariant(), email.Value);
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("not-an-email")]
    [DataRow("test@")]
    [ExpectedException(typeof(DomainException))]
    public void Create_InvalidEmail_ShouldThrow(string invalid)
    {
        Email.Create(invalid);
    }

    [TestMethod]
    public void Equals_SameEmail_ShouldReturnTrue()
    {
        var e1 = Email.Create("test@example.com");
        var e2 = Email.Create("TEST@example.com");

        Assert.AreEqual(e1, e2);
    }
}
