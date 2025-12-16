using FluentValidation;

namespace ProductService.Application.Features.Products.Queries.GetProductsByCategory;

public class GetProductsByCategoryQueryValidator : AbstractValidator<GetProductsByCategoryQuery>
{
    public GetProductsByCategoryQueryValidator()
    {
        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("CategoryId is required");
    }
}
