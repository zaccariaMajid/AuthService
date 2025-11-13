using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Domain.Common;
using AuthService.Domain.Exceptions;

namespace AuthService.Domain.ValueObjects;

public class Email : ValueObject
{
    public string Value { get; }

    private Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Email cannot be empty.", nameof(value));
        if (!value.Contains("@"))
            throw new DomainException("Invalid email format.", nameof(value));
        Value = value.Trim().ToLowerInvariant();
    }
    public static Email Create(string value) => new Email(value);
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
