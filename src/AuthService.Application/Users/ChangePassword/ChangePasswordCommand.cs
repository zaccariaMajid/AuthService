using AuthService.Application.Interfaces;
using AuthService.Domain.Common.Results;

namespace AuthService.Application.Users.ChangePassword;

public record ChangePasswordCommand(
    Guid UserId, 
    string CurrentPassword, 
    string NewPassword
) : ICommand<Result>;
