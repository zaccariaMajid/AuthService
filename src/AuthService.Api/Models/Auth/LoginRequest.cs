using System.ComponentModel.DataAnnotations;

namespace AuthService.Api.Models.Auth;

public record LoginRequest(
    [property: Required, EmailAddress] string Email,
    [property: Required] string Password);
