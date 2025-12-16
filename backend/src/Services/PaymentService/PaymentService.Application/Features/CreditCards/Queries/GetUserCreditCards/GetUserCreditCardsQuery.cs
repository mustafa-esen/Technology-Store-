using MediatR;
using PaymentService.Application.DTOs;

namespace PaymentService.Application.Features.CreditCards.Queries.GetUserCreditCards;

public class GetUserCreditCardsQuery : IRequest<List<CreditCardDto>>
{
    public string UserId { get; set; } = string.Empty;
}
