using AuthService.Application.Interfaces;

namespace AuthService.Application.Users.RegisterUser;

public record RegisterUserResponse(
    Guid UserId, 
    string Email
) : ICommand<Guid>;
