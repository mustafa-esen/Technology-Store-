using FluentValidation;

namespace BasketService.Application.Features.Baskets.Queries.GetBasket;

public class GetBasketQueryValidator : AbstractValidator<GetBasketQuery>
{
    public GetBasketQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");
    }
}
