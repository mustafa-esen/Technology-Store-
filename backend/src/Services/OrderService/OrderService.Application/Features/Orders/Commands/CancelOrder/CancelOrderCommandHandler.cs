using MediatR;
using MassTransit;
using Microsoft.Extensions.Logging;
using OrderService.Application.Interfaces;
using TechnologyStore.Shared.Events.Orders;

namespace OrderService.Application.Features.Orders.Commands.CancelOrder;

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, bool>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<CancelOrderCommandHandler> _logger;

    public CancelOrderCommandHandler(
        IOrderRepository orderRepository,
        IPublishEndpoint publishEndpoint,
        ILogger<CancelOrderCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task<bool> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Attempting to cancel order. OrderId: {OrderId}, Reason: {Reason}",
            request.OrderId, request.CancellationReason);

        var order = await _orderRepository.GetByIdWithItemsAsync(request.OrderId);
        if (order == null)
        {
            _logger.LogWarning("Order not found. OrderId: {OrderId}", request.OrderId);
            return false;
        }

        if (!order.CanBeCancelled())
        {
            _logger.LogWarning("Order cannot be cancelled. OrderId: {OrderId}, CurrentStatus: {Status}",
                order.Id, order.Status);
            return false;
        }

        _logger.LogInformation("📦 Order has {ItemCount} items to restore stock", order.Items.Count);

        order.Cancel(request.CancellationReason);
        await _orderRepository.UpdateAsync(order);

        _logger.LogInformation("Order cancelled successfully. OrderId: {OrderId}", order.Id);

        // Publish IOrderCancelledEvent with order items for stock restoration
        await _publishEndpoint.Publish<IOrderCancelledEvent>(new
        {
            OrderId = order.Id,
            UserId = order.UserId,
            Reason = request.CancellationReason,
            CancelledDate = DateTime.UtcNow,
            Items = order.Items.Select(item => new
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                Price = item.Price
            }).ToList()
        }, cancellationToken);

        _logger.LogInformation("✅ OrderCancelledEvent published. OrderId: {OrderId}", order.Id);

        return true;
    }
}
