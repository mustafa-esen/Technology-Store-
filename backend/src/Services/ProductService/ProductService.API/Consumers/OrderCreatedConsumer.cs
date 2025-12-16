using MassTransit;
using MediatR;
using ProductService.Application.Features.Products.Commands.DecreaseStock;
using TechnologyStore.Shared.Events.Orders;

namespace ProductService.API.Consumers;

/// Sipariş oluşturulduğunda ürün stoklarını düşürür
/// Consume: IOrderCreatedEvent (order-created-queue)
public class OrderCreatedConsumer : IConsumer<IOrderCreatedEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrderCreatedConsumer> _logger;

    public OrderCreatedConsumer(IMediator mediator, ILogger<OrderCreatedConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<IOrderCreatedEvent> context)
    {
        var message = context.Message;

        _logger.LogInformation("🐰 OrderCreatedEvent received for OrderId: {OrderId}, Items: {ItemCount}",
            message.OrderId, message.Items.Count);

        var failedProducts = new List<string>();

        // Her ürün için stok düşür
        foreach (var item in message.Items)
        {
            _logger.LogInformation("📦 Processing stock decrease for Product: {ProductName} (ID: {ProductId}), Quantity: {Quantity}",
                item.ProductName, item.ProductId, item.Quantity);

            var command = new DecreaseProductStockCommand
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
            };

            var success = await _mediator.Send(command);

            if (!success)
            {
                failedProducts.Add($"{item.ProductName} (ID: {item.ProductId})");
                _logger.LogWarning("⚠️ Failed to decrease stock for product: {ProductName}", item.ProductName);
            }
        }

        if (failedProducts.Any())
        {
            _logger.LogWarning("❌ Stock decrease completed with {FailureCount} failures: {FailedProducts}",
                failedProducts.Count, string.Join(", ", failedProducts));
        }
        else
        {
            _logger.LogInformation("✅ All stocks decreased successfully for OrderId: {OrderId}", message.OrderId);
        }
    }
}
