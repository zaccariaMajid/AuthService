using AuthService.Api.Models.Users;
using AuthService.Application;
using AuthService.Application.Users.AddRoleToUser;
using AuthService.Domain.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Api.Controllers;

[Route("api/[controller]")]
public class UsersController : ApiControllerBase
{
    private readonly ICommandHandler<AddRoleToUserCommand, Result> _addRoleHandler;

    public UsersController(ICommandHandler<AddRoleToUserCommand, Result> addRoleHandler)
    {
        _addRoleHandler = addRoleHandler;
    }

    [HttpPost("{userId:guid}/roles")]
    public async Task<IActionResult> AddRole(Guid userId, [FromBody] AssignRoleRequest request, CancellationToken cancellationToken)
    {
        var command = new AddRoleToUserCommand(userId, request.RoleId);
        var result = await _addRoleHandler.HandleAsync(command, cancellationToken);
        return ToActionResult(result);
    }
}
