using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.Domain.Common.Results;

public sealed record Error(string Code, string Message)
{
    public static Error Unknown => new Error("unknown_error", "An unknown error has occurred.");
    public static Error None => new Error(string.Empty, string.Empty);

    public override string ToString() => $"{Code}: {Message}";
}
