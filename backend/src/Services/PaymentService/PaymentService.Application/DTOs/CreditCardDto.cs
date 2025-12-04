namespace PaymentService.Application.DTOs;

public record CreditCardDto(
    Guid Id,
    string CardHolderName,
    string CardNumber,
    string ExpiryMonth,
    string ExpiryYear,
    string CardType,
    bool IsDefault,
    DateTime CreatedDate
);

public record CreateCreditCardDto(
    string CardHolderName,
    string CardNumber,
    string ExpiryMonth,
    string ExpiryYear
);

public record UpdateCreditCardDto(
    string CardHolderName,
    string ExpiryMonth,
    string ExpiryYear
);
