namespace AuthService.Application.Users.RefreshToken;

public record RefreshTokenResponse(
    Guid UserId,
    string AccessToken,
    string RefreshToken
);
