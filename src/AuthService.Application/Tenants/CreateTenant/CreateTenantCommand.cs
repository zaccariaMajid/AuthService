using AuthService.Application.Interfaces;
using AuthService.Domain.Common.Results;

namespace AuthService.Application.Tenants.CreateTenant;

public record CreateTenantCommand(
    string Name,
    string? Description) : ICommand<Result<CreateTenantResponse>>;
