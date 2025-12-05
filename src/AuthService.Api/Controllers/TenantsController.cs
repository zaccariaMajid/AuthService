using AuthService.Api.Models.Tenants;
using AuthService.Application;
using AuthService.Application.Tenants.AssignProductToTenant;
using AuthService.Application.Tenants.ActivateTenant;
using AuthService.Application.Tenants.CreateTenant;
using AuthService.Application.Tenants.DeactivateTenant;
using AuthService.Domain.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Api.Controllers;

[Route("api/[controller]")]
public class TenantsController : ApiControllerBase
{
    private readonly ICommandHandler<CreateTenantCommand, Result<CreateTenantResponse>> _createTenantHandler;
    private readonly ICommandHandler<ActivateTenantCommand, Result> _activateTenantHandler;
    private readonly ICommandHandler<DeactivateTenantCommand, Result> _deactivateTenantHandler;
    private readonly ICommandHandler<AssignProductToTenantCommand, Result> _assignProductHandler;

    public TenantsController(
        ICommandHandler<CreateTenantCommand, Result<CreateTenantResponse>> createTenantHandler,
        ICommandHandler<ActivateTenantCommand, Result> activateTenantHandler,
        ICommandHandler<DeactivateTenantCommand, Result> deactivateTenantHandler,
        ICommandHandler<AssignProductToTenantCommand, Result> assignProductHandler)
    {
        _createTenantHandler = createTenantHandler;
        _activateTenantHandler = activateTenantHandler;
        _deactivateTenantHandler = deactivateTenantHandler;
        _assignProductHandler = assignProductHandler;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTenantRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateTenantCommand(request.Name, request.Description);
        var result = await _createTenantHandler.HandleAsync(command, cancellationToken);
        return ToActionResult(result);
    }

    [HttpPost("activate")]
    public async Task<IActionResult> Activate([FromBody] UpdateTenantStatusRequest request, CancellationToken cancellationToken)
    {
        var command = new ActivateTenantCommand(request.TenantId);
        var result = await _activateTenantHandler.HandleAsync(command, cancellationToken);
        return ToActionResult(result);
    }

    [HttpPost("deactivate")]
    public async Task<IActionResult> Deactivate([FromBody] UpdateTenantStatusRequest request, CancellationToken cancellationToken)
    {
        var command = new DeactivateTenantCommand(request.TenantId);
        var result = await _deactivateTenantHandler.HandleAsync(command, cancellationToken);
        return ToActionResult(result);
    }

    [HttpPost("{tenantId:guid}/products")]
    public async Task<IActionResult> AssignProduct(Guid tenantId, [FromBody] AssignProductToTenantRequest request, CancellationToken cancellationToken)
    {
        var command = new AssignProductToTenantCommand(tenantId, request.ProductId);
        var result = await _assignProductHandler.HandleAsync(command, cancellationToken);
        return ToActionResult(result);
    }
}
