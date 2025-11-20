using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Domain.Common;
using AuthService.Domain.Exceptions;

namespace AuthService.Domain.ValueObjects;

public class PasswordHash : ValueObject
{
    public string Hash { get; }
    public string Salt { get; }

    public PasswordHash(string hash, string salt)
    {
        if (string.IsNullOrWhiteSpace(hash))
            throw new DomainException("Password hash cannot be null or empty.", nameof(hash));
        if (string.IsNullOrWhiteSpace(salt))
            throw new DomainException("Salt cannot be null or empty.", nameof(salt));

        Hash = hash;
        Salt = salt;
    }

    public static PasswordHash Create(string hash, string salt)
    {
        return new PasswordHash(hash, salt);
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Hash;
    }
}
