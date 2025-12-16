using FluentValidation;

namespace ProductService.Application.Features.Products.Commands.DecreaseStock;

public class DecreaseProductStockCommandValidator : AbstractValidator<DecreaseProductStockCommand>
{
    public DecreaseProductStockCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("ProductId is required.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero.")
            .LessThanOrEqualTo(1000)
            .WithMessage("Quantity cannot exceed 1000 items per order.");
    }
}
