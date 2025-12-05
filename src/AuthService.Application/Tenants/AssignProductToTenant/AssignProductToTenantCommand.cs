using AuthService.Application.Interfaces;
using AuthService.Domain.Common.Results;

namespace AuthService.Application.Tenants.AssignProductToTenant;

public record AssignProductToTenantCommand(Guid TenantId, Guid ProductId) : ICommand<Result>;
