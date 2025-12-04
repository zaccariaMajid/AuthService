using System.ComponentModel.DataAnnotations;

namespace AuthService.Api.Models.Tenants;

public record CreateTenantRequest(
    [property: Required] string Name,
    string? Description);
