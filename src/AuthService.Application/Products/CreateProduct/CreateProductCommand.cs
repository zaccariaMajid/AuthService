using AuthService.Application.Interfaces;
using AuthService.Domain.Common.Results;

namespace AuthService.Application.Products.CreateProduct;

public record CreateProductCommand(string Name, string? Description) : ICommand<Result<CreateProductResponse>>;
