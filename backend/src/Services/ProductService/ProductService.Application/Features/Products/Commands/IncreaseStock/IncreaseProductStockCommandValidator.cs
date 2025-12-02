using FluentValidation;

namespace ProductService.Application.Features.Products.Commands.IncreaseStock;

public class IncreaseProductStockCommandValidator : AbstractValidator<IncreaseProductStockCommand>
{
    public IncreaseProductStockCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("ProductId is required.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than zero.")
            .LessThanOrEqualTo(1000)
            .WithMessage("Quantity cannot exceed 1000.");
    }
}
