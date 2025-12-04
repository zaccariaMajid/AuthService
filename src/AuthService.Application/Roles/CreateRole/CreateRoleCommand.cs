using AuthService.Application.Interfaces;
using AuthService.Domain.Common.Results;

namespace AuthService.Application.Roles.CreateRole;

public record CreateRoleCommand(
    string Name,
    string Description,
    Guid TenantId
) : ICommand<Result<CreateRoleResponse>>;
