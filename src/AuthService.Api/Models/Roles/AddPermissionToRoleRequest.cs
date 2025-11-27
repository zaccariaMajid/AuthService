using System.ComponentModel.DataAnnotations;

namespace AuthService.Api.Models.Roles;

public record AddPermissionToRoleRequest([property: Required] Guid PermissionId);
