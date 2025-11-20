using AuthService.Application.Interfaces;
using AuthService.Domain.Common.Results;

namespace AuthService.Application.Roles.AddPermissionToRole;

public record AddPermissionToRoleCommand(
    Guid RoleId,
    Guid PermissionId
) : ICommand<Result>;
