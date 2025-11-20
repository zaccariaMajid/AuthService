using AuthService.Application.Interfaces;
using AuthService.Domain.Common.Results;

namespace AuthService.Application.Users.AddRoleToUser;

public record AddRoleToUserCommand(
    Guid UserId, 
    Guid RoleId
) : ICommand<Result>;
