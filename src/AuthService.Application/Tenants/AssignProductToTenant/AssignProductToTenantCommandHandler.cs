using AuthService.Application.Interfaces.Repositories;
using AuthService.Domain.Common.Results;

namespace AuthService.Application.Tenants.AssignProductToTenant;

public class AssignProductToTenantCommandHandler : ICommandHandler<AssignProductToTenantCommand, Result>
{
    private readonly ITenantRepository _tenants;
    private readonly IProductRepository _products;

    public AssignProductToTenantCommandHandler(ITenantRepository tenants, IProductRepository products)
    {
        _tenants = tenants;
        _products = products;
    }

    public async Task<Result> HandleAsync(AssignProductToTenantCommand command, CancellationToken cancellationToken)
    {
        var tenant = await _tenants.GetByIdWithProductsAsync(command.TenantId);
        if (tenant is null)
            return Result.Failure(new Error("Tenant.NotFound", "Tenant not found."));

        var product = await _products.GetByIdAsync(command.ProductId);
        if (product is null)
            return Result.Failure(new Error("Product.NotFound", "Product not found."));

        tenant.AddProduct(product);
        await _tenants.UpdateAsync(tenant);

        return Result.Success();
    }
}
