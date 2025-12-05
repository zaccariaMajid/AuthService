using System.ComponentModel.DataAnnotations;

namespace AuthService.Api.Models.Products;

public record CreateProductRequest(
    [property: Required] string Name,
    string? Description);
