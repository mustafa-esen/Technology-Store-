using BasketService.Application.DTOs;
using BasketService.Application.Interfaces;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using TechnologyStore.Shared.Events.Baskets;

namespace BasketService.Application.Features.Baskets.Commands.CheckoutBasket;

public class CheckoutBasketCommandHandler : IRequestHandler<CheckoutBasketCommand, bool>
{
    private readonly IBasketRepository _basketRepository;
    private readonly IProductServiceClient _productServiceClient;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<CheckoutBasketCommandHandler> _logger;

    public CheckoutBasketCommandHandler(
        IBasketRepository basketRepository,
        IProductServiceClient productServiceClient,
        IPublishEndpoint publishEndpoint,
        ILogger<CheckoutBasketCommandHandler> logger)
    {
        _basketRepository = basketRepository;
        _productServiceClient = productServiceClient;
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

        _logger.LogInformation("📦 Checking stock availability before checkout...");

        var stockCheckRequest = new StockCheckRequest
        {
            Items = basket.Items.Select(item => new StockCheckItemDto
            {
                ProductId = item.ProductId,
                RequiredQuantity = item.Quantity
            }).ToList()
        };

        var stockCheckResult = await _productServiceClient.CheckStockAsync(stockCheckRequest, cancellationToken);

        if (!stockCheckResult.IsAvailable)
        {
            _logger.LogWarning("❌ Stock check failed. {IssueCount} items have insufficient stock",
                stockCheckResult.Issues.Count);

            foreach (var issue in stockCheckResult.Issues)
            {
                _logger.LogWarning("⚠️ Product: {ProductName}, Required: {Required}, Available: {Available}",
                    issue.ProductName, issue.RequiredQuantity, issue.AvailableStock);
            }

            throw new InvalidOperationException(
                $"Insufficient stock: {string.Join(", ", stockCheckResult.Issues.Select(i => $"{i.ProductName} (need {i.RequiredQuantity}, have {i.AvailableStock})"))}"
            );
        }

        _logger.LogInformation("✅ Stock check passed. Proceeding with checkout...");

        // Sepet event'ini yayınla
        await _publishEndpoint.Publish<IBasketCheckoutEvent>(new
        {
            UserId = request.UserId,
            UserName = request.UserName,
            TotalPrice = basket.TotalPrice,
            ShippingAddress = request.ShippingAddress,
            Items = basket.Items.Select(item => new TechnologyStore.Shared.Events.Baskets.BasketItemDto
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
