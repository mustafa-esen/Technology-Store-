using BasketService.Application.Interfaces;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using TechnologyStore.Shared.Events.Baskets;

namespace BasketService.Application.Features.Baskets.Commands.CheckoutBasket;

public class CheckoutBasketCommandHandler : IRequestHandler<CheckoutBasketCommand, bool>
{
    private readonly IBasketRepository _basketRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<CheckoutBasketCommandHandler> _logger;

    public CheckoutBasketCommandHandler(
        IBasketRepository basketRepository,
        IPublishEndpoint publishEndpoint,
        ILogger<CheckoutBasketCommandHandler> logger)
    {
        _basketRepository = basketRepository;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task<bool> Handle(CheckoutBasketCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("🛒 Processing basket checkout for user: {UserId}", request.UserId);

        var basket = await _basketRepository.GetBasketAsync(request.UserId);

        if (basket == null)
        {
            _logger.LogWarning("⚠️ Basket not found for user: {UserId}", request.UserId);
            return false;
        }

        if (basket.Items.Count == 0)
        {
            _logger.LogWarning("⚠️ Cannot checkout empty basket for user: {UserId}", request.UserId);
            return false;
        }

        // Sepet event'ini yayınla
        await _publishEndpoint.Publish<IBasketCheckoutEvent>(new
        {
            UserId = request.UserId,
            UserName = request.UserName,
            TotalPrice = basket.TotalPrice,
            ShippingAddress = request.ShippingAddress,
            Items = basket.Items.Select(item => new BasketItemDto
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                Price = item.Price
            }).ToList(),
            CheckedOutDate = DateTime.UtcNow
        }, cancellationToken);

        _logger.LogInformation("✅ Basket checkout event published for user: {UserId}, Total: {Total:C}",
            request.UserId, basket.TotalPrice);

        // Sepeti temizle
        var deleted = await _basketRepository.DeleteBasketAsync(request.UserId);

        if (deleted)
        {
            _logger.LogInformation("🗑️ Basket cleared after checkout for user: {UserId}", request.UserId);
        }

        return true;
    }
}
