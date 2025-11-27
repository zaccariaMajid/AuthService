using System.ComponentModel.DataAnnotations;

namespace AuthService.Api.Models.Auth;

public record ChangePasswordRequest(
    [property: Required] Guid UserId,
    [property: Required] string CurrentPassword,
    [property: Required] string NewPassword);
