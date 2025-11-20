using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Domain.AggregateRoots;
using AuthService.Domain.Common.Results;
using AuthService.Domain.Interfaces;
using AuthService.Domain.ValueObjects;

namespace AuthService.Application.Users.RegisterUser;

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
        if (string.IsNullOrWhiteSpace(command.Password))
            return Result<RegisterUserResponse>.Failure(new Error("User.PasswordEmpty", "Password must be at least 6 characters long."));

        var exists = await _userRepository.GetByEmailAsync(command.Email) is not null;
        if (exists)
            return Result<RegisterUserResponse>.Failure(new Error("User.EmailTaken", "A user with the given email already exists."));

        var salt = _passwordHasher.GenerateSalt();
        var hash = _passwordHasher.Hash(command.Password, salt);

        var email = Email.Create(command.Email);

        var passwordHash = PasswordHash.Create(hash, salt);

        var user = User.Create(email, passwordHash, firstName: command.firstName, lastName: command.lastName);

        await _userRepository.AddAsync(user);

        return Result<RegisterUserResponse>.Success(
            new RegisterUserResponse(user.Id, user.Email.Value));
    }
}
