using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Application.Interfaces;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Domain.AggregateRoots;
using AuthService.Domain.Interfaces;
using AuthService.Domain.ValueObjects;

namespace AuthService.Application.Users;

public record RegisterUserResponse(Guid UserId, string Email) : ICommand<Guid>;
public record RegisterUserCommand(string firstName, string lastName, string Password, string Email) : ICommand<RegisterUserResponse>;

public class RegisterUserCommandHandler :
    ICommandHandler<RegisterUserCommand, RegisterUserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<RegisterUserResponse> HandleAsync(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        var exists = _userRepository.GetByEmailAsync(command.Email) is null;
        if (exists)
            throw new InvalidOperationException("User with the given email already exists.");
        var salt = _passwordHasher.GenerateSalt();
        var hashedPassword = _passwordHasher.HashPassword(command.Password, salt);

        var email = Email.Create(command.Email);

        var user = User.Create(email, hashedPassword, firstName: command.firstName, lastName: command.lastName);

        await _userRepository.AddAsync(user);

        return new RegisterUserResponse(user.Id, user.Email.Value);
    }
}
