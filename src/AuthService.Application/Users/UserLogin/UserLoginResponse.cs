namespace AuthService.Application.Users.UserLogin;

public record UserLoginResponse(
    Guid UserId,
    string Token,
    string RefreshToken,
    string AccessToken);
