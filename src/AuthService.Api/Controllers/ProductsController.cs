using AuthService.Api.Models.Products;
using AuthService.Application;
using AuthService.Application.Products.CreateProduct;
using AuthService.Domain.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Api.Controllers;

[Route("api/[controller]")]
public class ProductsController : ApiControllerBase
{
    private readonly ICommandHandler<CreateProductCommand, Result<CreateProductResponse>> _createProductHandler;

    public ProductsController(ICommandHandler<CreateProductCommand, Result<CreateProductResponse>> createProductHandler)
    {
        _createProductHandler = createProductHandler;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateProductCommand(request.Name, request.Description);
        var result = await _createProductHandler.HandleAsync(command, cancellationToken);
        return ToActionResult(result);
    }
}
