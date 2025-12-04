using System.ComponentModel.DataAnnotations;

namespace AuthService.Api.Models.Roles;

public record CreateRoleRequest(
    [property: Required] string Name,
    string? Description,
    [property: Required] Guid TenantId);
