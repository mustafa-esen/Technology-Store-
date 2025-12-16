using FluentValidation;

namespace BasketService.Application.Features.Baskets.Commands.ClearBasket;

public class ClearBasketCommandValidator : AbstractValidator<ClearBasketCommand>
{
    public ClearBasketCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");
    }
}
