using BasketService.Application.Features.Baskets.Commands.AddItemToBasket;
using BasketService.Application.Features.Baskets.Commands.ClearBasket;
using BasketService.Application.Features.Baskets.Commands.RemoveItemFromBasket;
using BasketService.Application.Features.Baskets.Commands.UpdateItemQuantity;
using BasketService.Application.Features.Baskets.Queries.GetBasket;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BasketService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BasketsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<BasketsController> _logger;

    public BasketsController(IMediator mediator, ILogger<BasketsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetBasket(string userId)
    {
        _logger.LogInformation("🛒 GET request received for basket: {UserId}", userId);

        var query = new GetBasketQuery { UserId = userId };
        var result = await _mediator.Send(query);

        if (result == null)
        {
            _logger.LogInformation("📭 Basket not found for user: {UserId}", userId);
            return NotFound(new { message = "Basket not found" });
        }

        _logger.LogInformation("✅ Basket retrieved successfully for user: {UserId}", userId);
        return Ok(result);
    }

    [HttpPost("{userId}/items")]
    public async Task<IActionResult> AddItemToBasket(string userId, [FromBody] AddItemToBasketCommand command)
    {
        _logger.LogInformation("➕ POST request to add item to basket for user: {UserId}, Product: {ProductId}",
            userId, command.ProductId);

        command.UserId = userId;
        var result = await _mediator.Send(command);

        if (result)
        {
            _logger.LogInformation("✅ Item added to basket successfully for user: {UserId}", userId);
            return Ok(new { message = "Item added to basket successfully" });
        }

        _logger.LogWarning("⚠️ Failed to add item to basket for user: {UserId}", userId);
        return BadRequest(new { message = "Failed to add item to basket" });
    }

    [HttpDelete("{userId}/items/{productId}")]
    public async Task<IActionResult> RemoveItemFromBasket(string userId, Guid productId)
    {
        _logger.LogInformation("➖ DELETE request to remove item from basket for user: {UserId}, Product: {ProductId}",
            userId, productId);

        var command = new RemoveItemFromBasketCommand
        {
            UserId = userId,
            ProductId = productId
        };

        var result = await _mediator.Send(command);

        if (result)
        {
            _logger.LogInformation("✅ Item removed from basket successfully for user: {UserId}", userId);
            return Ok(new { message = "Item removed from basket successfully" });
        }

        _logger.LogWarning("⚠️ Basket or item not found for user: {UserId}, Product: {ProductId}", userId, productId);
        return NotFound(new { message = "Basket or item not found" });
    }

    [HttpPut("{userId}/items/{productId}")]
    public async Task<IActionResult> UpdateItemQuantity(string userId, Guid productId, [FromBody] UpdateItemQuantityRequest request)
    {
        _logger.LogInformation("🔄 PUT request to update item quantity for user: {UserId}, Product: {ProductId}, NewQuantity: {Quantity}",
            userId, productId, request.Quantity);

        var command = new UpdateItemQuantityCommand
        {
            UserId = userId,
            ProductId = productId,
            Quantity = request.Quantity
        };

        var result = await _mediator.Send(command);

        if (result)
        {
            _logger.LogInformation("✅ Item quantity updated successfully for user: {UserId}", userId);
            return Ok(new { message = "Item quantity updated successfully" });
        }

        _logger.LogWarning("⚠️ Basket or item not found for user: {UserId}, Product: {ProductId}", userId, productId);
        return NotFound(new { message = "Basket or item not found" });
    }

    [HttpDelete("{userId}")]
    public async Task<IActionResult> ClearBasket(string userId)
    {
        _logger.LogInformation("🗑️ DELETE request to clear basket for user: {UserId}", userId);

        var command = new ClearBasketCommand { UserId = userId };
        var result = await _mediator.Send(command);

        if (result)
        {
            _logger.LogInformation("✅ Basket cleared successfully for user: {UserId}", userId);
            return Ok(new { message = "Basket cleared successfully" });
        }

        _logger.LogWarning("⚠️ Basket not found for user: {UserId}", userId);
        return NotFound(new { message = "Basket not found" });
    }
}

public record UpdateItemQuantityRequest(int Quantity);
