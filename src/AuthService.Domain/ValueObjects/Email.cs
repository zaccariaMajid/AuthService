using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
        var trimmed = value.Trim();
        var pattern = @"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$";
        if (!Regex.IsMatch(trimmed, pattern, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase))
            throw new DomainException("Invalid email format.", nameof(value));
        Value = trimmed.ToLowerInvariant();
    }
    public static Email Create(string value) => new Email(value);
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}
