using AuthService.Application.Interfaces;
using AuthService.Domain.Common.Results;

namespace AuthService.Application.Users.RefreshToken;

public record RefreshTokenCommand(string RefreshToken)
    : ICommand<Result<RefreshTokenResponse>>;
