using System.ComponentModel.DataAnnotations;

namespace AuthService.Api.Models.Tenants;

public record AssignProductToTenantRequest([property: Required] Guid ProductId);
