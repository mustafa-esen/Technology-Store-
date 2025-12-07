using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.DTOs;
using OrderService.Application.Features.Orders.Commands.CancelOrder;
using OrderService.Application.Features.Orders.Commands.CreateOrder;
using OrderService.Application.Features.Orders.Commands.UpdateOrderStatus;
using OrderService.Application.Features.Orders.Queries.GetAllOrders;
using OrderService.Application.Features.Orders.Queries.GetOrder;
using OrderService.Application.Features.Orders.Queries.GetUserOrders;

namespace OrderService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IMediator mediator, ILogger<OrdersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto dto)
    {
        _logger.LogInformation("Creating new order for user: {UserId}", dto.UserId);

        var command = new CreateOrderCommand
        {
            UserId = dto.UserId,
            Items = dto.Items,
            ShippingAddress = dto.ShippingAddress,
            Notes = dto.Notes
        };

        var result = await _mediator.Send(command);

        if (result == null)
        {
            _logger.LogWarning("Order creation failed for user: {UserId}", dto.UserId);
            return BadRequest("Failed to create order");
        }

        _logger.LogInformation("Order created successfully. OrderId: {OrderId}", result.Id);
        return CreatedAtAction(nameof(GetOrder), new { id = result.Id }, result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetOrder(Guid id)
    {
        _logger.LogInformation("Getting order: {OrderId}", id);

        var query = new GetOrderQuery { OrderId = id };
        var result = await _mediator.Send(query);

        if (result == null)
        {
            _logger.LogWarning("Order not found: {OrderId}", id);
            return NotFound();
        }

        return Ok(result);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<List<OrderDto>>> GetUserOrders(string userId)
    {
        _logger.LogInformation("Getting orders for user: {UserId}", userId);

        var query = new GetUserOrdersQuery { UserId = userId };
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    [HttpGet("all")]
    public async Task<ActionResult<List<OrderDto>>> GetAllOrders()
    {
        _logger.LogInformation("Admin getting all orders");

        var query = new GetAllOrdersQuery();
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    [HttpPut("{id}/status")]
    public async Task<ActionResult> UpdateOrderStatus(Guid id, [FromBody] UpdateOrderStatusDto dto)
    {
        _logger.LogInformation("Updating order status. OrderId: {OrderId}, NewStatus: {Status}", id, dto.Status);

        var command = new UpdateOrderStatusCommand
        {
            OrderId = id,
            Status = dto.Status
        };

        var result = await _mediator.Send(command);

        if (!result)
        {
            _logger.LogWarning("Failed to update order status. OrderId: {OrderId}", id);
            return NotFound("Order not found or status update failed");
        }

        return NoContent();
    }

    [HttpPut("{id}/cancel")]
    public async Task<ActionResult> CancelOrder(Guid id, [FromBody] CancelOrderDto dto)
    {
        _logger.LogInformation("Cancelling order. OrderId: {OrderId}", id);

        var command = new CancelOrderCommand
        {
            OrderId = id,
            CancellationReason = dto.Reason
        };

        var result = await _mediator.Send(command);

        if (!result)
        {
            _logger.LogWarning("Failed to cancel order. OrderId: {OrderId}", id);
            return BadRequest("Order not found or cannot be cancelled");
        }

        return NoContent();
    }
}
