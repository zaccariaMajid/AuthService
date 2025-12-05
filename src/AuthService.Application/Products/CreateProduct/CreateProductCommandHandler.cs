using AuthService.Application.Interfaces.Repositories;
using AuthService.Domain.AggregateRoots;
using AuthService.Domain.Common.Results;

namespace AuthService.Application.Products.CreateProduct;

public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, Result<CreateProductResponse>>
{
    private readonly IProductRepository _products;

    public CreateProductCommandHandler(IProductRepository products)
    {
        _products = products;
    }

    public async Task<Result<CreateProductResponse>> HandleAsync(CreateProductCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
            return Result<CreateProductResponse>.Failure(new Error("Product.InvalidName", "Product name cannot be empty."));

        var exists = await _products.GetByNameAsync(command.Name.Trim()) is not null;
        if (exists)
            return Result<CreateProductResponse>.Failure(new Error("Product.Exists", "A product with this name already exists."));

        var product = Product.Create(command.Name.Trim(), command.Description);
        await _products.AddAsync(product);

        return Result<CreateProductResponse>.Success(new CreateProductResponse(product.Id, product.Name));
    }
}
