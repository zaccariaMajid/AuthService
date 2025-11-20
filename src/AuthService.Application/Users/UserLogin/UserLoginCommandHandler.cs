using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Domain.AggregateRoots;
using AuthService.Domain.Common.Results;
using AuthService.Domain.Interfaces;

namespace AuthService.Application.Users.UserLogin;

public class UserLoginCommandHandler : 
    ICommandHandler<UserLoginCommand, Result<UserLoginResponse>>
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;
    private readonly ITokenService _tokenService;

    public UserLoginCommandHandler(
        IUserRepository users,
        IPasswordHasher hasher,
        ITokenService tokenService)
    {
        _users = users;
        _hasher = hasher;
        _tokenService = tokenService;
    }
    
    public async Task<Result<UserLoginResponse>> HandleAsync(UserLoginCommand command, CancellationToken cancellationToken)
    {
        if(string.IsNullOrWhiteSpace(command.Email))
            return Result<UserLoginResponse>.Failure(new Error("invalid_email", "Email cannot be empty."));

        if(string.IsNullOrWhiteSpace(command.Password))
            return Result<UserLoginResponse>.Failure(new Error("invalid_password", "Password cannot be empty."));


        var user = await _users.GetByEmailAsync(command.Email);

        if (user is null)
            return Result<UserLoginResponse>.Failure(new Error("user_not_found", "No user found with the given email."));

        var isPasswordValid = _hasher.VerifyPassword(user.PasswordHash.Hash, command.Password, user.PasswordHash.Salt);

        if (!isPasswordValid)
            return Result<UserLoginResponse>.Failure(new Error("invalid_credentials", "The provided credentials are incorrect."));

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken(user.Id);

        return Result<UserLoginResponse>.Success(
            new UserLoginResponse(user.Id, accessToken, refreshToken.Token, accessToken));

    }
}
