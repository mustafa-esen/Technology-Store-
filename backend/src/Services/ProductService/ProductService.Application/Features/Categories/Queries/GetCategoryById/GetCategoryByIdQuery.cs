using MediatR;
using ProductService.Application.DTOs;

namespace ProductService.Application.Features.Categories.Queries.GetCategoryById;

public record GetCategoryByIdQuery(Guid Id) : IRequest<CategoryDto?>;
