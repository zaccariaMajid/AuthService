using AuthService.Api.Models.Permissions;
using AuthService.Application;
using AuthService.Application.Permissions.CreatePermission;
using AuthService.Domain.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Api.Controllers;

[Route("api/[controller]")]
public class PermissionsController : ApiControllerBase
{
    private readonly ICommandHandler<CreatePermissionCommand, Result<CreatePermissionResponse>> _createPermissionHandler;

    public PermissionsController(ICommandHandler<CreatePermissionCommand, Result<CreatePermissionResponse>> createPermissionHandler)
    {
        _createPermissionHandler = createPermissionHandler;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePermissionRequest request, CancellationToken cancellationToken)
    {
        var command = new CreatePermissionCommand(request.Name, request.Description ?? string.Empty);
        var result = await _createPermissionHandler.HandleAsync(command, cancellationToken);
        return ToActionResult(result);
    }
}
