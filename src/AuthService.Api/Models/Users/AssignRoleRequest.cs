using System.ComponentModel.DataAnnotations;

namespace AuthService.Api.Models.Users;

public record AssignRoleRequest([property: Required] Guid RoleId);
