using AuthService.Application.Interfaces.Repositories;
using AuthService.Domain.Common.Results;

namespace AuthService.Application.Tenants.ActivateTenant;

public class ActivateTenantCommandHandler : ICommandHandler<ActivateTenantCommand, Result>
{
    private readonly ITenantRepository _tenants;

    public ActivateTenantCommandHandler(ITenantRepository tenants)
    {
        _tenants = tenants;
    }

    public async Task<Result> HandleAsync(ActivateTenantCommand command, CancellationToken cancellationToken)
    {
        var tenant = await _tenants.GetByIdAsync(command.TenantId);
        if (tenant is null)
            return Result.Failure(new Error("Tenant.NotFound", "Tenant not found."));

        tenant.Activate();
        await _tenants.UpdateAsync(tenant);

        return Result.Success();
    }
}
