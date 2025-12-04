using AuthService.Application.Interfaces;
using AuthService.Domain.Common.Results;

namespace AuthService.Application.Tenants.ActivateTenant;

public record ActivateTenantCommand(Guid TenantId) : ICommand<Result>;
