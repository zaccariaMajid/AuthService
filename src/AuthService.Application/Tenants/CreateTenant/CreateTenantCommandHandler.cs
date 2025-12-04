using AuthService.Application.Interfaces.Repositories;
using AuthService.Domain.AggregateRoots;
using AuthService.Domain.Common.Results;

namespace AuthService.Application.Tenants.CreateTenant;

public class CreateTenantCommandHandler : ICommandHandler<CreateTenantCommand, Result<CreateTenantResponse>>
{
    private readonly ITenantRepository _tenants;

    public CreateTenantCommandHandler(ITenantRepository tenants)
    {
        _tenants = tenants;
    }

    public async Task<Result<CreateTenantResponse>> HandleAsync(CreateTenantCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
            return Result<CreateTenantResponse>.Failure(new Error("Tenant.InvalidName", "Tenant name cannot be empty."));

        var exists = await _tenants.GetByNameAsync(command.Name.Trim()) is not null;
        if (exists)
            return Result<CreateTenantResponse>.Failure(new Error("Tenant.Exists", "A tenant with this name already exists."));

        var tenant = Tenant.Create(command.Name.Trim(), command.Description);
        await _tenants.AddAsync(tenant);

        return Result<CreateTenantResponse>.Success(new CreateTenantResponse(tenant.Id, tenant.Name));
    }
}
