using MassTransit;
using MediatR;
using ProductService.Application.Features.Products.Commands.IncreaseStock;
using TechnologyStore.Shared.Events.Orders;

namespace ProductService.API.Consumers;

/// Sipariş iptal edildiğinde ürün stoklarını geri yükler
/// Consume: IOrderCancelledEvent (order-cancelled-queue)
public class OrderCancelledConsumer : IConsumer<IOrderCancelledEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrderCancelledConsumer> _logger;

    public OrderCancelledConsumer(IMediator mediator, ILogger<OrderCancelledConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<IOrderCancelledEvent> context)
    {
        var message = context.Message;

        _logger.LogInformation("🐰 OrderCancelledEvent received for OrderId: {OrderId}, Items: {ItemCount}",
            message.OrderId, message.Items.Count);

        var failedProducts = new List<string>();

        // Her ürün için stok geri yükle
        foreach (var item in message.Items)
        {
            _logger.LogInformation("📦 Processing stock increase for Product: {ProductName} (ID: {ProductId}), Quantity: {Quantity}",
                item.ProductName, item.ProductId, item.Quantity);

            var command = new IncreaseProductStockCommand
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
            };

            var success = await _mediator.Send(command);

            if (!success)
            {
                failedProducts.Add($"{item.ProductName} (ID: {item.ProductId})");
                _logger.LogWarning("⚠️ Failed to increase stock for product: {ProductName}", item.ProductName);
            }
        }

        if (failedProducts.Any())
        {
            _logger.LogWarning("⚠️ Order cancellation completed with {FailedCount} failed stock increases: {FailedProducts}",
                failedProducts.Count, string.Join(", ", failedProducts));
        }
        else
        {
            _logger.LogInformation("✅ All stocks successfully restored for cancelled order: {OrderId}", message.OrderId);
        }
    }
}
