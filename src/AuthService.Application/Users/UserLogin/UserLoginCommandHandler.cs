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
    private readonly ITenantRepository _tenants;

    public UserLoginCommandHandler(
        IUserRepository users,
        IPasswordHasher hasher,
        ITokenService tokenService,
        ITenantRepository tenants)
    {
        _users = users;
        _hasher = hasher;
        _tokenService = tokenService;
        _tenants = tenants;
    }

    public async Task<Result<UserLoginResponse>> HandleAsync(UserLoginCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Email))
            return Result<UserLoginResponse>.Failure(new Error("User.InvalidEmail", "Email cannot be empty."));

        if (string.IsNullOrWhiteSpace(command.Password))
            return Result<UserLoginResponse>.Failure(new Error("User.InvalidPassword", "Password cannot be empty."));


        if (command.TenantId == Guid.Empty)
            return Result<UserLoginResponse>.Failure(new Error("User.InvalidTenant", "Tenant is required."));

        var tenant = await _tenants.GetByIdAsync(command.TenantId);
        if (tenant is null || !tenant.IsActive)
            return Result<UserLoginResponse>.Failure(new Error("User.TenantUnavailable", "Tenant not found or inactive."));

        var user = await _users.GetByEmailAsync(command.Email, command.TenantId);

        if (user is null)
            return Result<UserLoginResponse>.Failure(new Error("User.NotFound", "No user found with the given email."));

        var isPasswordValid = _hasher.VerifyPassword(user.PasswordHash.Hash, command.Password, user.PasswordHash.Salt);

        if (!isPasswordValid)
            return Result<UserLoginResponse>.Failure(new Error("User.InvalidCredentials", "The provided credentials are incorrect."));

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken(user.Id);

        return Result<UserLoginResponse>.Success(
            new UserLoginResponse(user.Id, accessToken, refreshToken.Token, accessToken));

    }
}
