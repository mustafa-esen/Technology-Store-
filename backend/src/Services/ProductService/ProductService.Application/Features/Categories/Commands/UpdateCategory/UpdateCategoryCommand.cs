using MediatR;

namespace ProductService.Application.Features.Categories.Commands.UpdateCategory;

public record UpdateCategoryCommand(
    Guid Id,
    string Name,
    string Description,
    bool IsActive
) : IRequest<bool>;
