using FluentValidation;

namespace PaymentService.Application.Features.CreditCards.Commands.UpdateCreditCard;

public class UpdateCreditCardCommandValidator : AbstractValidator<UpdateCreditCardCommand>
{
    public UpdateCreditCardCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Card ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.CardHolderName)
            .NotEmpty()
            .WithMessage("Card holder name is required")
            .MaximumLength(100)
            .WithMessage("Card holder name must not exceed 100 characters");

        RuleFor(x => x.ExpiryMonth)
            .NotEmpty()
            .WithMessage("Expiry month is required")
            .Matches(@"^(0[1-9]|1[0-2])$")
            .WithMessage("Expiry month must be in MM format (01-12)");

        RuleFor(x => x.ExpiryYear)
            .NotEmpty()
            .WithMessage("Expiry year is required")
            .Matches(@"^\d{2}$")
            .WithMessage("Expiry year must be in YY format");
    }
}
