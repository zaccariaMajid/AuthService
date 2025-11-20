using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Domain.AggregateRoots;
using AuthService.Domain.Common.Results;

namespace AuthService.Application.Permissions.CreatePermission;

public class CreatePermissionCommandHandler :
    ICommandHandler<CreatePermissionCommand, Result<CreatePermissionResponse>>
{
    private readonly IPermissionRepository _permissions;

    public CreatePermissionCommandHandler(IPermissionRepository permissions)
    {
        _permissions = permissions;
    }
    public async Task<Result<CreatePermissionResponse>> HandleAsync(CreatePermissionCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
            return Result<CreatePermissionResponse>.Failure(new Error("Permission.InvalidName", "Permission name cannot be empty"));

        var exists = await _permissions.GetByNameAsync(command.Name) is not null;

        if (exists)
            return Result<CreatePermissionResponse>.Failure(new Error("Permission.Exists", "A permission with this name already exists"));

        var permission = Permission.Create(command.Name, command.Description);
        await _permissions.AddAsync(permission);

        return Result<CreatePermissionResponse>.Success(new CreatePermissionResponse(permission.Id, permission.Name));
    }
}
