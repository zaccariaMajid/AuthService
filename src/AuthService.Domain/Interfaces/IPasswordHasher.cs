using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.Domain.Interfaces;

public interface IPasswordHasher
{
    string Hash(string password, string salt);
    bool VerifyPassword(string hashedPassword, string providedPassword, string salt);
    string GenerateSalt();
}
