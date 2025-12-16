using FluentValidation;

namespace PaymentService.Application.Features.CreditCards.Commands.CreateCreditCard;

public class CreateCreditCardCommandValidator : AbstractValidator<CreateCreditCardCommand>
{
    public CreateCreditCardCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.CardHolderName)
            .NotEmpty()
            .WithMessage("Card holder name is required")
            .MaximumLength(100)
            .WithMessage("Card holder name cannot exceed 100 characters");

        RuleFor(x => x.CardNumber)
            .NotEmpty()
            .WithMessage("Card number is required")
            .Matches(@"^\d{13,19}$")
            .WithMessage("Card number must be between 13 and 19 digits");

        RuleFor(x => x.ExpiryMonth)
            .InclusiveBetween(1, 12)
            .WithMessage("Expiry month must be between 1 and 12");

        RuleFor(x => x.ExpiryYear)
            .GreaterThanOrEqualTo(DateTime.Now.Year % 100)
            .WithMessage("Expiry year cannot be in the past");

        RuleFor(x => x.Cvv)
            .NotEmpty()
            .WithMessage("CVV is required")
            .Matches(@"^\d{3,4}$")
            .WithMessage("CVV must be 3 or 4 digits");
    }
}
