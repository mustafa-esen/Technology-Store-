using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using PaymentService.Application.DTOs;
using PaymentService.Application.Interfaces;
using PaymentService.Domain.Entities;

namespace PaymentService.Application.Features.CreditCards.Commands.CreateCreditCard;

public class CreateCreditCardCommandHandler : IRequestHandler<CreateCreditCardCommand, CreditCardDto>
{
    private readonly ICreditCardRepository _creditCardRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateCreditCardCommandHandler> _logger;

    public CreateCreditCardCommandHandler(
        ICreditCardRepository creditCardRepository,
        IMapper mapper,
        ILogger<CreateCreditCardCommandHandler> _logger)
    {
        _creditCardRepository = creditCardRepository;
        _mapper = mapper;
        this._logger = _logger;
    }

    public async Task<CreditCardDto> Handle(CreateCreditCardCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("💳 Creating credit card for user: {UserId}", request.UserId);

        // Convert int to string with leading zero for month
        var expiryMonth = request.ExpiryMonth.ToString("D2");
        var expiryYear = request.ExpiryYear.ToString("D2");

        var creditCard = new CreditCard(
            request.UserId,
            request.CardHolderName,
            request.CardNumber,
            expiryMonth,
            expiryYear
        );

        // Handle default card logic
        var existingCards = await _creditCardRepository.GetByUserIdAsync(request.UserId);

        if (request.IsDefault || existingCards.Count == 0)
        {
            // If user wants this as default or it's their first card, set as default
            // and remove default from all other cards
            foreach (var card in existingCards)
            {
                card.RemoveDefault();
                await _creditCardRepository.UpdateAsync(card);
            }

            creditCard.SetAsDefault();
            _logger.LogInformation("✅ Card set as default");
        }

        var created = await _creditCardRepository.AddAsync(creditCard);

        _logger.LogInformation("✅ Credit card created: {CardType} ending {LastFour}",
            created.CardType, created.CardNumber.Substring(created.CardNumber.Length - 4));

        return _mapper.Map<CreditCardDto>(created);
    }
}
