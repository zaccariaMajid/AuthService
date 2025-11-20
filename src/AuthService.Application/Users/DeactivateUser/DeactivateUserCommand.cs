using AuthService.Application.Interfaces;
using AuthService.Domain.Common.Results;

namespace AuthService.Application.Users.DeactivateUser;

public record DeactivateUserCommand(Guid UserId) :
    ICommand<Result>;
