using AuthService.Api.Models.Auth;
using AuthService.Application;
using AuthService.Application.Users.ActivateUser;
using AuthService.Application.Users.ChangePassword;
using AuthService.Application.Users.DeactivateUser;
using AuthService.Application.Users.RefreshToken;
using AuthService.Application.Users.RegisterUser;
using AuthService.Application.Users.UserLogin;
using AuthService.Domain.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Api.Controllers;

[Route("api/[controller]")]
public class AuthController : ApiControllerBase
{
    private readonly ICommandHandler<RegisterUserCommand, Result<RegisterUserResponse>> _registerHandler;
    private readonly ICommandHandler<UserLoginCommand, Result<UserLoginResponse>> _loginHandler;
    private readonly ICommandHandler<RefreshTokenCommand, Result<RefreshTokenResponse>> _refreshTokenHandler;
    private readonly ICommandHandler<ChangePasswordCommand, Result> _changePasswordHandler;
    private readonly ICommandHandler<ActivateUserCommand, Result> _activateHandler;
    private readonly ICommandHandler<DeactivateUserCommand, Result> _deactivateHandler;

    public AuthController(
        ICommandHandler<RegisterUserCommand, Result<RegisterUserResponse>> registerHandler,
        ICommandHandler<UserLoginCommand, Result<UserLoginResponse>> loginHandler,
        ICommandHandler<RefreshTokenCommand, Result<RefreshTokenResponse>> refreshTokenHandler,
        ICommandHandler<ChangePasswordCommand, Result> changePasswordHandler,
        ICommandHandler<ActivateUserCommand, Result> activateHandler,
        ICommandHandler<DeactivateUserCommand, Result> deactivateHandler)
    {
        _registerHandler = registerHandler;
        _loginHandler = loginHandler;
        _refreshTokenHandler = refreshTokenHandler;
        _changePasswordHandler = changePasswordHandler;
        _activateHandler = activateHandler;
        _deactivateHandler = deactivateHandler;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(request.FirstName, request.LastName, request.Password, request.Email);
        var result = await _registerHandler.HandleAsync(command, cancellationToken);
        return ToActionResult(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var command = new UserLoginCommand(request.Email, request.Password);
        var result = await _loginHandler.HandleAsync(command, cancellationToken);
        return ToActionResult(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var command = new RefreshTokenCommand(request.RefreshToken);
        var result = await _refreshTokenHandler.HandleAsync(command, cancellationToken);
        return ToActionResult(result);
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var command = new ChangePasswordCommand(request.UserId, request.CurrentPassword, request.NewPassword);
        var result = await _changePasswordHandler.HandleAsync(command, cancellationToken);
        return ToActionResult(result);
    }

    [HttpPost("activate")]
    public async Task<IActionResult> Activate([FromBody] UpdateUserStatusRequest request, CancellationToken cancellationToken)
    {
        var command = new ActivateUserCommand(request.UserId);
        var result = await _activateHandler.HandleAsync(command, cancellationToken);
        return ToActionResult(result);
    }

    [HttpPost("deactivate")]
    public async Task<IActionResult> Deactivate([FromBody] UpdateUserStatusRequest request, CancellationToken cancellationToken)
    {
        var command = new DeactivateUserCommand(request.UserId);
        var result = await _deactivateHandler.HandleAsync(command, cancellationToken);
        return ToActionResult(result);
    }
}
