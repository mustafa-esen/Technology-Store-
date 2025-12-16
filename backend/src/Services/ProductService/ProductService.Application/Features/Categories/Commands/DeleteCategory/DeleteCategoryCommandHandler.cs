using MediatR;
using ProductService.Application.Interfaces;

namespace ProductService.Application.Features.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, bool>
{
    private readonly ICategoryRepository _categoryRepository;

    public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        if (!await _categoryRepository.ExistsAsync(request.Id))
            return false;

        await _categoryRepository.DeleteAsync(request.Id);
        return true;
    }
}
