using AuthService.Application.Interfaces.Repositories;
using AuthService.Domain.Common.Results;

namespace AuthService.Application.Tenants.DeactivateTenant;

public class DeactivateTenantCommandHandler : ICommandHandler<DeactivateTenantCommand, Result>
{
    private readonly ITenantRepository _tenants;

    public DeactivateTenantCommandHandler(ITenantRepository tenants)
    {
        _tenants = tenants;
    }

    public async Task<Result> HandleAsync(DeactivateTenantCommand command, CancellationToken cancellationToken)
    {
        var tenant = await _tenants.GetByIdAsync(command.TenantId);
        if (tenant is null)
            return Result.Failure(new Error("Tenant.NotFound", "Tenant not found."));

        tenant.Deactivate();
        await _tenants.UpdateAsync(tenant);

        return Result.Success();
    }
}
