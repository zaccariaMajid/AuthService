using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Domain.Common.Results;

namespace AuthService.Application.Users.DeactivateUser;

public class DeactivateUserCommandHandler :
    ICommandHandler<DeactivateUserCommand, Result>
{
    private readonly IUserRepository _users;

    public DeactivateUserCommandHandler(IUserRepository users)
    {
        _users = users;
    }
    public async Task<Result> HandleAsync(DeactivateUserCommand command, CancellationToken cancellationToken)
    {
        var user = await _users.GetByIdAsync(command.UserId);

        if (user is null)
            return Result.Failure(new Error("User.NotFound", "User not found"));

        if (user.IsActive)
            return Result.Failure(new Error("User.AlreadyActive", "User is already active"));

        user.Deactivate();
        await _users.UpdateAsync(user);

        return Result.Success();
    }
}
