using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using AuthService.Application.Interfaces.Repositories;
using AuthService.Domain.Common.Results;
using AuthService.Domain.Interfaces;

namespace AuthService.Application.Users.RefreshToken;
public class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, Result<RefreshTokenResponse>>
{
    private readonly IRefreshTokenRepository _refreshTokens;
    private readonly IUserRepository _users;
    private readonly ITokenService _tokenService;

    public RefreshTokenCommandHandler(
        IRefreshTokenRepository refreshTokens,
        IUserRepository users,
        ITokenService tokenService)
    {
        _refreshTokens = refreshTokens;
        _users = users;
        _tokenService = tokenService;
    }
    public async Task<Result<RefreshTokenResponse>> HandleAsync(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var existingToken = await _refreshTokens.GetByTokenAsync(command.RefreshToken);
        if (existingToken is null || existingToken.RevokedAt != null)
            return Result<RefreshTokenResponse>.Failure(new Error("invalid_refresh_token", "The provided refresh token is invalid."));

        if (existingToken.ExpiresAt < DateTime.UtcNow)
            return Result<RefreshTokenResponse>.Failure(new Error("expired_refresh_token", "The provided refresh token has expired."));

        var user = await _users.GetByIdAsync(existingToken.UserId);
        if (user is null)
            return Result<RefreshTokenResponse>.Failure(new Error("user_not_found", "No user found for the given refresh token."));

        var newAccessToken = _tokenService.GenerateAccessToken(user);
        var newRefreshToken = _tokenService.GenerateRefreshToken(user.Id);

        return Result<RefreshTokenResponse>.Success(
            new RefreshTokenResponse(newAccessToken, newRefreshToken.Token));
    }
}
