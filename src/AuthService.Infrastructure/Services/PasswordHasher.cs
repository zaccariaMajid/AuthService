using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using AuthService.Domain.Interfaces;

namespace AuthService.Infrastructure.Services;

public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password, string salt)
    {
        var value = Rfc2898DeriveBytes.Pbkdf2(
            password,
            Convert.FromBase64String(salt),
            100000,
            HashAlgorithmName.SHA256,
            32);

        return Convert.ToBase64String(value);
    }

    public bool VerifyPassword(string password, string hash, string salt)
    {
        var computed = Hash(password, salt);
        return hash == computed;
    }

    public string GenerateSalt()
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        return Convert.ToBase64String(salt);
    }

}