using AuthService.Api.Models.Roles;
using AuthService.Application;
using AuthService.Application.Roles.AddPermissionToRole;
using AuthService.Application.Roles.CreateRole;
using AuthService.Domain.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Api.Controllers;

[Route("api/[controller]")]
public class RolesController : ApiControllerBase
{
    private readonly ICommandHandler<CreateRoleCommand, Result<CreateRoleResponse>> _createRoleHandler;
    private readonly ICommandHandler<AddPermissionToRoleCommand, Result> _addPermissionHandler;

    public RolesController(
        ICommandHandler<CreateRoleCommand, Result<CreateRoleResponse>> createRoleHandler,
        ICommandHandler<AddPermissionToRoleCommand, Result> addPermissionHandler)
    {
        _createRoleHandler = createRoleHandler;
        _addPermissionHandler = addPermissionHandler;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRoleRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateRoleCommand(request.Name, request.Description ?? string.Empty, request.TenantId);
        var result = await _createRoleHandler.HandleAsync(command, cancellationToken);
        return ToActionResult(result);
    }

    [HttpPost("{roleId:guid}/permissions")]
    public async Task<IActionResult> AddPermission(Guid roleId, [FromBody] AddPermissionToRoleRequest request, CancellationToken cancellationToken)
    {
        var command = new AddPermissionToRoleCommand(roleId, request.PermissionId);
        var result = await _addPermissionHandler.HandleAsync(command, cancellationToken);
        return ToActionResult(result);
    }
}
