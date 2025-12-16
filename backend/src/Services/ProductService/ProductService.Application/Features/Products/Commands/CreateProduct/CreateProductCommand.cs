using MediatR;
using ProductService.Application.DTOs;

namespace ProductService.Application.Features.Products.Commands.CreateProduct;

public record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    int Stock,
    Guid CategoryId,
    string Brand,
    string ImageUrl
) : IRequest<ProductDto>;
