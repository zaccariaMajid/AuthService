using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Domain.Common.Results;

namespace AuthService.Application.Users.ActivateUser;
public class ActivateUserCommandHandler :
    ICommandHandler<ActivateUserCommand, Result>
{
    private readonly IUserRepository _users;

    public ActivateUserCommandHandler(IUserRepository users)
    {
        _users = users;
    }
    public async Task<Result> HandleAsync(ActivateUserCommand command, CancellationToken cancellationToken)
    {
        var user = await _users.GetByIdAsync(command.UserId);

        if (user is null)
            return Result.Failure(new Error("User.NotFound", "User not found"));

        if (user.IsActive)
            return Result.Failure(new Error("User.AlreadyActive", "User is already active"));

        user.Activate();
        await _users.UpdateAsync(user);

        return Result.Success();
    }
}
