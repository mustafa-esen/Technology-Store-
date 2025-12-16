using FluentValidation;

namespace PaymentService.Application.Features.Payments.Commands.ProcessPayment;

public class ProcessPaymentCommandValidator : AbstractValidator<ProcessPaymentCommand>
{
    public ProcessPaymentCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("OrderId is required")
            .MaximumLength(100).WithMessage("OrderId cannot exceed 100 characters");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required")
            .MaximumLength(100).WithMessage("UserId cannot exceed 100 characters");

        RuleFor(x => x.TotalAmount)
            .GreaterThan(0).WithMessage("TotalAmount must be greater than 0");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required")
            .Length(3).WithMessage("Currency must be 3 characters (e.g., TRY, USD, EUR)");
    }
}
