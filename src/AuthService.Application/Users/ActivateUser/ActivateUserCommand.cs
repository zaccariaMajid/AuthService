using AuthService.Application.Interfaces;
using AuthService.Domain.Common.Results;

namespace AuthService.Application.Users.ActivateUser;

public record ActivateUserCommand(Guid UserId) :
    ICommand<Result>;
