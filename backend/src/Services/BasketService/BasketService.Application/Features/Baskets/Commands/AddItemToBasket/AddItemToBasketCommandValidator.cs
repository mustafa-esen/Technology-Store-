using FluentValidation;

namespace BasketService.Application.Features.Baskets.Commands.AddItemToBasket;

public class AddItemToBasketCommandValidator : AbstractValidator<AddItemToBasketCommand>
{
    public AddItemToBasketCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("ProductId is required");

        RuleFor(x => x.ProductName)
            .NotEmpty().WithMessage("ProductName is required")
            .MaximumLength(200).WithMessage("ProductName must not exceed 200 characters");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0");
    }
}
