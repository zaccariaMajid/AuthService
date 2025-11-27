using System.ComponentModel.DataAnnotations;

namespace AuthService.Api.Models.Permissions;

public record CreatePermissionRequest(
    [property: Required] string Name,
    string? Description);
