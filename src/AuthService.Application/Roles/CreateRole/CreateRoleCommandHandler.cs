using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Domain.AggregateRoots;
using AuthService.Domain.Common.Results;

namespace AuthService.Application.Roles.CreateRole;
public class CreateRoleCommandHandler :
    ICommandHandler<CreateRoleCommand, Result<CreateRoleResponse>>
{
    private readonly IRoleRepository _roles;
    private readonly ITenantRepository _tenants;

    public CreateRoleCommandHandler(IRoleRepository roles, ITenantRepository tenants)
    {
        _roles = roles;
        _tenants = tenants;
    }
    public async Task<Result<CreateRoleResponse>> HandleAsync(CreateRoleCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
            return Result<CreateRoleResponse>.Failure(new Error("Role.InvalidName", "Role name cannot be empty"));

        if (command.TenantId == Guid.Empty)
            return Result<CreateRoleResponse>.Failure(new Error("Role.InvalidTenant", "Tenant is required"));

        var tenant = await _tenants.GetByIdAsync(command.TenantId);
        if (tenant is null || !tenant.IsActive)
            return Result<CreateRoleResponse>.Failure(new Error("Role.TenantUnavailable", "Tenant not found or inactive"));

        var exists = await _roles.GetByNameAsync(command.Name, command.TenantId) is not null;
        if (exists)
            return Result<CreateRoleResponse>.Failure(new Error("Role.Exists", "A role with this name already exists"));

        var role = Role.Create(command.Name, command.Description, tenantId: command.TenantId);

        await _roles.AddAsync(role);

        return Result<CreateRoleResponse>.Success(new CreateRoleResponse(role.Id, role.Name));
    }
}
