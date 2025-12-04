using AuthService.Application.Interfaces;
using AuthService.Domain.Common.Results;

namespace AuthService.Application.Tenants.DeactivateTenant;

public record DeactivateTenantCommand(Guid TenantId) : ICommand<Result>;
