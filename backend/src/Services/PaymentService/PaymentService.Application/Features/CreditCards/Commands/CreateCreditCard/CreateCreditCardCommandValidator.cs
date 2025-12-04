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
            .NotEmpty()
            .WithMessage("Expiry month is required")
            .Matches(@"^(0[1-9]|1[0-2])$")
            .WithMessage("Expiry month must be between 01 and 12");

        RuleFor(x => x.ExpiryYear)
            .NotEmpty()
            .WithMessage("Expiry year is required")
            .Matches(@"^\d{2}$")
            .WithMessage("Expiry year must be 2 digits (YY)");
    }
}
