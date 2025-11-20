using AuthService.Application.Interfaces;
using AuthService.Domain.Common.Results;

namespace AuthService.Application.Users.UserLogin;

public record UserLoginCommand(
    string Email,
    string Password
) : ICommand<Result<UserLoginResponse>>;
