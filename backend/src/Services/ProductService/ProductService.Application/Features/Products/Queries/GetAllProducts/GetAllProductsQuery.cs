using MediatR;
using ProductService.Application.DTOs;

namespace ProductService.Application.Features.Products.Queries.GetAllProducts;

public record GetAllProductsQuery : IRequest<IEnumerable<ProductDto>>;
