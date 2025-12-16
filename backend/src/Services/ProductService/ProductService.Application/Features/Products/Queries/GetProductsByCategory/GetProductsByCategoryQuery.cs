using MediatR;
using ProductService.Application.DTOs;

namespace ProductService.Application.Features.Products.Queries.GetProductsByCategory;

public record GetProductsByCategoryQuery(Guid CategoryId) : IRequest<IEnumerable<ProductDto>>;
