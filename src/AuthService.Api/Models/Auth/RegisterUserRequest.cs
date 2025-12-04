using System.ComponentModel.DataAnnotations;

namespace AuthService.Api.Models.Auth;

public record RegisterUserRequest(
    [property: Required] string FirstName,
    [property: Required] string LastName,
    [property: Required, EmailAddress] string Email,
    [property: Required] string Password,
    [property: Required] Guid TenantId);
