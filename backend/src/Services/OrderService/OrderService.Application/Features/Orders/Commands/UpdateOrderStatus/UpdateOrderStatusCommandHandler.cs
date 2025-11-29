using MediatR;
using MassTransit;
using Microsoft.Extensions.Logging;
using OrderService.Application.Interfaces;
using OrderService.Domain.Enums;
using TechnologyStore.Shared.Events.Orders;

namespace OrderService.Application.Features.Orders.Commands.UpdateOrderStatus;

public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, bool>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<UpdateOrderStatusCommandHandler> _logger;

    public UpdateOrderStatusCommandHandler(
        IOrderRepository orderRepository,
        IPublishEndpoint publishEndpoint,
        ILogger<UpdateOrderStatusCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task<bool> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating order status. OrderId: {OrderId}, NewStatus: {Status}",
            request.OrderId, request.Status);

        var order = await _orderRepository.GetByIdAsync(request.OrderId);
        if (order == null)
        {
            _logger.LogWarning("Order not found. OrderId: {OrderId}", request.OrderId);
            return false;
        }

        var oldStatus = order.Status;
        var newStatus = Enum.Parse<Domain.Enums.OrderStatus>(request.Status, true);

        order.UpdateStatus(newStatus);
        await _orderRepository.UpdateAsync(order);

        _logger.LogInformation("Order status updated successfully. OrderId: {OrderId}, OldStatus: {OldStatus}, NewStatus: {NewStatus}",
            order.Id, oldStatus, newStatus);

        // Publish OrderStatusChangedEvent
        var statusChangedEvent = new OrderStatusChangedEvent
        {
            OrderId = order.Id,
            UserId = order.UserId,
            OldStatus = (TechnologyStore.Shared.Events.Orders.OrderStatus)Enum.Parse(typeof(TechnologyStore.Shared.Events.Orders.OrderStatus), oldStatus.ToString()),
            NewStatus = (TechnologyStore.Shared.Events.Orders.OrderStatus)Enum.Parse(typeof(TechnologyStore.Shared.Events.Orders.OrderStatus), newStatus.ToString()),
            ChangedDate = DateTime.UtcNow
        };

        await _publishEndpoint.Publish(statusChangedEvent, cancellationToken);
        _logger.LogInformation("OrderStatusChangedEvent published. OrderId: {OrderId}", order.Id);

        // If order is completed, publish OrderCompletedEvent
        if (newStatus == Domain.Enums.OrderStatus.Delivered)
        {
            var completedEvent = new OrderCompletedEvent
            {
                OrderId = order.Id,
                UserId = order.UserId,
                TotalAmount = order.TotalAmount,
                CompletedDate = DateTime.UtcNow
            };

            await _publishEndpoint.Publish(completedEvent, cancellationToken);
            _logger.LogInformation("OrderCompletedEvent published. OrderId: {OrderId}", order.Id);
        }

        return true;
    }
}
