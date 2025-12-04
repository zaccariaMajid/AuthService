using System.ComponentModel.DataAnnotations;

namespace AuthService.Api.Models.Tenants;

public record UpdateTenantStatusRequest([property: Required] Guid TenantId);
