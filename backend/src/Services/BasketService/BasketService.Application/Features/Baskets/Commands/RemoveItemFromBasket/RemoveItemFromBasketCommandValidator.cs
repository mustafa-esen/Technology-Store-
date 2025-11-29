using FluentValidation;

namespace BasketService.Application.Features.Baskets.Commands.RemoveItemFromBasket;

public class RemoveItemFromBasketCommandValidator : AbstractValidator<RemoveItemFromBasketCommand>
{
    public RemoveItemFromBasketCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");

        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("ProductId is required");
    }
}
