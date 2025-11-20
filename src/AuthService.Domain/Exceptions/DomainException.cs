using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.Domain.Exceptions;

public class DomainException : Exception
{
    public string? Name { get; }

    public DomainException(string message)
        : base(message) { }

    public DomainException(string message, string? name)
        : base(message)
    {
        Name = name;
    }

    public DomainException(string message, Exception inner)
        : base(message, inner) { }
}