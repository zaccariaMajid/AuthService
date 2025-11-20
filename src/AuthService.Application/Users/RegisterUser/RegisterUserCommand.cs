using AuthService.Application.Interfaces;
using AuthService.Domain.Common.Results;

namespace AuthService.Application.Users.RegisterUser;

public record RegisterUserCommand(
    string firstName, 
    string lastName, 
    string Password, 
    string Email
) : ICommand<Result<RegisterUserResponse>>;
