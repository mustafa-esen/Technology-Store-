using MediatR;
using ProductService.Application.Interfaces;

namespace ProductService.Application.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, bool>
{
    private readonly ICategoryRepository _categoryRepository;

    public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<bool> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id);
        if (category == null) return false;

        category.Name = request.Name;
        category.Description = request.Description;
        category.IsActive = request.IsActive;

        await _categoryRepository.UpdateAsync(category);
        return true;
    }
}
