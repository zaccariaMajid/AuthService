using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Domain.Common.Results;
using AuthService.Domain.Interfaces;

namespace AuthService.Application.Users.ChangePassword;
public class ChangePasswordCommandHandler :
    ICommandHandler<ChangePasswordCommand, Result>
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;

    public ChangePasswordCommandHandler(
        IUserRepository users,
        IPasswordHasher hasher)
    {
        _users = users;
        _hasher = hasher;
    }
    public async Task<Result> HandleAsync(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        var user =  await _users.GetByIdAsync(command.UserId);
        if (user is null)
            return Result.Failure(new Error("user_not_found", "No user found with the given ID."));
        
        var isCurrentPasswordValid = _hasher.VerifyPassword(user.PasswordHash.Hash, command.CurrentPassword, user.PasswordHash.Salt);
        if (!isCurrentPasswordValid)
            return Result.Failure(new Error("invalid_current_password", "The current password is incorrect."));

        var newSalt = _hasher.GenerateSalt();
        var newHash = _hasher.Hash(command.NewPassword, newSalt);

        var newPasswordHash = Domain.ValueObjects.PasswordHash.Create(newHash, newSalt);
        user.ChangePassword(newPasswordHash);

        await _users.UpdateAsync(user);

        return Result.Success();
    }
}
