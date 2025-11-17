using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthService.Domain.Common.Results;

public class Result<T> : Result
{
    public T Value { get; init; } = default!;

    protected Result(bool isSuccess, T value, Error error)
        : base(isSuccess, error)
    {
        if (isSuccess && value == null)
            throw new InvalidOperationException("A successful result must have a value.");

        Value = value!;
    }

    public static Result<T> Success(T value)
        => new(true, value, Error.None);

    public new static Result<T> Failure(Error error)
        => new(false, default!, error);
}
