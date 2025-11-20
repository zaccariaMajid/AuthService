using AuthService.Application.Interfaces;
using AuthService.Domain.Common.Results;

namespace AuthService.Application.Permissions.CreatePermission;

public record CreatePermissionCommand(
    string Name,
    string Description
) : ICommand<Result<CreatePermissionResponse>>;
