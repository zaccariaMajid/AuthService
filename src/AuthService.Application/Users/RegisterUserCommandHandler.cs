using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Application.Interfaces;
using AuthService.Domain.Interfaces;

namespace AuthService.Application.Users;

public record RegisterUserResponse(Guid UserId, string Email) : ICommand<Guid>;
public record RegisterUserCommand(string Username, string Password, string Email) : ICommand<RegisterUserResponse>;

public class RegisterUserCommandHandler :
    ICommandHandler<RegisterUserCommand, RegisterUserResponse>
{
    // private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserCommandHandler(
        // IUserRepository userRepository,
        IPasswordHasher passwordHasher)
    {
        // _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public Task<RegisterUserResponse> HandleAsync(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
