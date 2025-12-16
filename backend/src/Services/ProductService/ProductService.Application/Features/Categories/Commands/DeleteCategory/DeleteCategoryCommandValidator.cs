using FluentValidation;

namespace ProductService.Application.Features.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
{
    public DeleteCategoryCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Kategori ID'si boş olamaz");
    }
}
