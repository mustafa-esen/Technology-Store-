using MediatR;

namespace ProductService.Application.Features.Products.Commands.UpdateProduct;

public record UpdateProductCommand(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    int Stock,
    Guid CategoryId,
    string Brand,
    string ImageUrl,
    bool IsActive
) : IRequest<bool>;
