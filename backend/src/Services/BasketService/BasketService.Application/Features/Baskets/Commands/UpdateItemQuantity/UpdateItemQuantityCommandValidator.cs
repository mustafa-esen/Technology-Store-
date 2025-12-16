using FluentValidation;

namespace BasketService.Application.Features.Baskets.Commands.UpdateItemQuantity;

public class UpdateItemQuantityCommandValidator : AbstractValidator<UpdateItemQuantityCommand>
{
    public UpdateItemQuantityCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");

        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("ProductId is required");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0).WithMessage("Quantity must be 0 or greater");
    }
}
