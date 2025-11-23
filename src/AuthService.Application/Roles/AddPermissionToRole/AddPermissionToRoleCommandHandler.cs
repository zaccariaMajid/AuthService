using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Domain.Common.Results;

namespace AuthService.Application.Roles.AddPermissionToRole;

public class AddPermissionToRoleCommandHandler :
    ICommandHandler<AddPermissionToRoleCommand, Result>
{
    private readonly IRoleRepository _roles;
    private readonly IPermissionRepository _permissions;

    public AddPermissionToRoleCommandHandler(IRoleRepository roles, IPermissionRepository permissions)
    {
        _roles = roles;
        _permissions = permissions;
    }
    public async Task<Result> HandleAsync(AddPermissionToRoleCommand command, CancellationToken cancellationToken)
    {
        var role = await _roles.GetByIdAsync(command.RoleId);
        if (role is null)
            return Result.Failure(new Error("Role.NotFound", "No roles found with the given id"));

        var permission = await _permissions.GetByIdAsync(command.PermissionId);
        if (permission is null)
            return Result.Failure(new Error("Permission.NotFound", "No Permissions found with the given id"));

        role.AddPermission(permission);
        await _roles.UpdateAsync(role);

        return Result.Success();
    }
}
