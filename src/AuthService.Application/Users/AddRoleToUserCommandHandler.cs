using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AuthService.Application.Interfaces;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Domain.Common.Results;

namespace AuthService.Application.Users;

public record AddRoleToUserCommand(Guid UserId, Guid RoleId) : ICommand<Result>;
public class AddRoleToUserCommandHandler :
    ICommandHandler<AddRoleToUserCommand, Result>
{
    private readonly IRoleRepository _roles;
    private readonly IUserRepository _users;

    public AddRoleToUserCommandHandler(IRoleRepository roles, IUserRepository users)
    {
        _roles = roles;
        _users = users;
    }
    public async Task<Result> HandleAsync(AddRoleToUserCommand command, CancellationToken cancellationToken)
    {
        var role = await _roles.GetByIdAsync(command.RoleId);
        if(role is null)
            return Result.Failure(new Error("Role.NotExists", "No roles found with the given id"));
        var user = await _users.GetByIdAsync(command.UserId);
        if(user is null)
            return Result.Failure(new Error("User.NotExists", "No user found with the given id"));

        user.AssignRole(role);
        await _users.UpdateAsync(user);

        return Result.Success();
        
    }
}
