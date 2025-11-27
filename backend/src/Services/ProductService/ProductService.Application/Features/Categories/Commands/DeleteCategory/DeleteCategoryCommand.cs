using MediatR;

namespace ProductService.Application.Features.Categories.Commands.DeleteCategory;

public record DeleteCategoryCommand(Guid Id) : IRequest<bool>;
