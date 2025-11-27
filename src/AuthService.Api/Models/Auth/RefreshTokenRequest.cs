using System.ComponentModel.DataAnnotations;

namespace AuthService.Api.Models.Auth;

public record RefreshTokenRequest([property: Required] string RefreshToken);
