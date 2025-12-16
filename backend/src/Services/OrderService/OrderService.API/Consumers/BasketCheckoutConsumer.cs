using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using OrderService.Application.Features.Orders.Commands.CreateOrder;
using TechnologyStore.Shared.Events.Baskets;

namespace OrderService.API.Consumers;

/// BasketService'ten gelen IBasketCheckoutEvent'i dinler
/// Consumer = Controller mantığı: Sadece event'i alıp Application katmanına (MediatR) iletir
/// İş mantığı burada değil, CreateOrderCommandHandler'da çalışır
public class BasketCheckoutConsumer : IConsumer<IBasketCheckoutEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<BasketCheckoutConsumer> _logger;

    public BasketCheckoutConsumer(IMediator mediator, ILogger<BasketCheckoutConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<IBasketCheckoutEvent> context)
    {
        var basketEvent = context.Message;

        _logger.LogInformation("🛒 Basket checkout event received for user: {UserId} ({UserName}), Total: {Total:C}",
            basketEvent.UserId, basketEvent.UserName, basketEvent.TotalPrice);

        try
        {
            // 1. Event verisini Command'e çevir (Factory method kullan)
            var command = CreateOrderCommand.FromBasketCheckoutEvent(basketEvent);

            // 2. İşi Application katmanına (MediatR Handler'a) devret
            // Consumer burada sadece bir postacıdır, iş mantığı CommandHandler'da
            var orderDto = await _mediator.Send(command);

            _logger.LogInformation("✅ Order created successfully from basket checkout. OrderId: {OrderId}, UserId: {UserId}",
                orderDto.Id, basketEvent.UserId);

            // Message başarıyla işlendi, RabbitMQ'ya ACK gönderilecek
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error processing basket checkout event for user: {UserId}", basketEvent.UserId);

            // Hata fırlat, RabbitMQ mesajı tekrar kuyruğa koyar (Retry)
            // Retry policy Program.cs'te yapılandırılmalı
            throw;
        }
    }
}
