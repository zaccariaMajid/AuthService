using System.ComponentModel.DataAnnotations;

namespace AuthService.Api.Models.Auth;

public record UpdateUserStatusRequest([property: Required] Guid UserId);
