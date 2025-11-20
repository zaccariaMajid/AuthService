using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Domain.AggregateRoots;
using AuthService.Domain.Common;
using AuthService.Domain.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AuthService.Domain.Tests;

[TestClass]
public class PermissionTests
{
    [TestMethod]
    public void CreatePermission_ShouldSetDataAndRaiseEvent()
    {
        var permission = Permission.Create("read:users", "Permission to read user data");

        Assert.AreEqual("read:users", permission.Name);
        Assert.AreEqual("Permission to read user data", permission.Description);
    }

    [TestMethod]
    [ExpectedException(typeof(DomainException))]
    public void CreatePermission_InvalidName_ShouldThrow()
    {
        Permission.Create("", "Permission with invalid name");
    }
}
