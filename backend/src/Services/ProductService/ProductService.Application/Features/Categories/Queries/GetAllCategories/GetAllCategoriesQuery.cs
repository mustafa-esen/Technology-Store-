using MediatR;
using ProductService.Application.DTOs;

namespace ProductService.Application.Features.Categories.Queries.GetAllCategories;

public record GetAllCategoriesQuery : IRequest<IEnumerable<CategoryDto>>;
