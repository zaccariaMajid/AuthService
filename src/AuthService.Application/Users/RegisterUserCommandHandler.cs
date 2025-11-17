using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Application.Interfaces;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Domain.AggregateRoots;
using AuthService.Domain.Common.Results;
using AuthService.Domain.Interfaces;
using AuthService.Domain.ValueObjects;

namespace AuthService.Application.Users;

public record RegisterUserResponse(Guid UserId, string Email) : ICommand<Guid>;
public record RegisterUserCommand(string firstName, string lastName, string Password, string Email) : 
    ICommand<Result<RegisterUserResponse>>;

public class RegisterUserCommandHandler :
    ICommandHandler<RegisterUserCommand, Result<RegisterUserResponse>>
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

    public async Task<Result<RegisterUserResponse>> HandleAsync(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        if(string.IsNullOrWhiteSpace(command.Password))
            return Result<RegisterUserResponse>.Failure(new Error("invalid_password", "Password must be at least 6 characters long."));

        var exists = _userRepository.GetByEmailAsync(command.Email) is null;
        if (exists)
            return Result<RegisterUserResponse>.Failure(new Error("user_already_exists", "A user with the given email already exists."));

        var salt = _passwordHasher.GenerateSalt();
        var hashedPassword = _passwordHasher.HashPassword(command.Password, salt);

        var email = Email.Create(command.Email);

        var user = User.Create(email, hashedPassword, firstName: command.firstName, lastName: command.lastName);

        await _userRepository.AddAsync(user);

        return Result<RegisterUserResponse>.Success(
            new RegisterUserResponse(user.Id, user.Email.Value));
    }
}
