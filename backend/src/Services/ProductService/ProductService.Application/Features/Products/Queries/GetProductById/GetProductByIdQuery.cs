using MediatR;
using ProductService.Application.DTOs;

namespace ProductService.Application.Features.Products.Queries.GetProductById;

public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto?>;
