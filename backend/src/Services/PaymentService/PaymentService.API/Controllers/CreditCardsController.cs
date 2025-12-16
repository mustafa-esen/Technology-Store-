using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Application.DTOs;
using PaymentService.Application.Features.CreditCards.Commands.CreateCreditCard;
using PaymentService.Application.Features.CreditCards.Commands.DeleteCreditCard;
using PaymentService.Application.Features.CreditCards.Commands.SetDefaultCard;
using PaymentService.Application.Features.CreditCards.Commands.UpdateCreditCard;
using PaymentService.Application.Features.CreditCards.Queries.GetCreditCardById;
using PaymentService.Application.Features.CreditCards.Queries.GetDefaultCard;
using PaymentService.Application.Features.CreditCards.Queries.GetUserCreditCards;
using System.Security.Claims;

namespace PaymentService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CreditCardsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CreditCardsController> _logger;

    public CreditCardsController(IMediator mediator, ILogger<CreditCardsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    private string GetUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
    }

    [HttpPost]
    public async Task<IActionResult> CreateCreditCard([FromBody] CreateCreditCardDto dto)
    {
        var userId = GetUserId();
        _logger.LogInformation("⚡ POST Create Credit Card for user: {UserId}", userId);

        var command = new CreateCreditCardCommand
        {
            UserId = userId,
            CardHolderName = dto.CardHolderName,
            CardNumber = dto.CardNumber,
            ExpiryMonth = dto.ExpiryMonth,
            ExpiryYear = dto.ExpiryYear,
            Cvv = dto.Cvv,
            IsDefault = dto.IsDefault
        };

        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetCreditCardById), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetUserCreditCards()
    {
        var userId = GetUserId();
        _logger.LogInformation("⚡ GET Credit Cards for user: {UserId}", userId);

        var query = new GetUserCreditCardsQuery { UserId = userId };
        var result = await _mediator.Send(query);

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCreditCardById(Guid id)
    {
        var userId = GetUserId();
        _logger.LogInformation("⚡ GET Credit Card by Id: {CardId} for user: {UserId}", id, userId);

        var query = new GetCreditCardByIdQuery { Id = id, UserId = userId };
        var result = await _mediator.Send(query);

        if (result == null)
        {
            _logger.LogWarning("⚠️ Credit card not found or unauthorized: {CardId}", id);
            return NotFound(new { Message = $"Credit card with Id {id} not found" });
        }

        return Ok(result);
    }

    [HttpGet("default")]
    public async Task<IActionResult> GetDefaultCard()
    {
        var userId = GetUserId();
        _logger.LogInformation("⚡ GET Default Credit Card for user: {UserId}", userId);

        var query = new GetDefaultCardQuery { UserId = userId };
        var result = await _mediator.Send(query);

        if (result == null)
        {
            _logger.LogWarning("⚠️ No default credit card found for user: {UserId}", userId);
            return NotFound(new { Message = "No default credit card found" });
        }

        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateCreditCard(Guid id, [FromBody] UpdateCreditCardDto dto)
    {
        var userId = GetUserId();
        _logger.LogInformation("⚡ PUT Update Credit Card: {CardId} for user: {UserId}", id, userId);

        var command = new UpdateCreditCardCommand
        {
            Id = id,
            UserId = userId,
            CardHolderName = dto.CardHolderName,
            ExpiryMonth = dto.ExpiryMonth,
            ExpiryYear = dto.ExpiryYear
        };

        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error updating credit card: {CardId}", id);
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPut("{id:guid}/set-default")]
    public async Task<IActionResult> SetDefaultCard(Guid id)
    {
        var userId = GetUserId();
        _logger.LogInformation("⚡ PUT Set Default Credit Card: {CardId} for user: {UserId}", id, userId);

        var command = new SetDefaultCardCommand { Id = id, UserId = userId };
        var result = await _mediator.Send(command);

        if (!result)
        {
            _logger.LogWarning("⚠️ Failed to set default card: {CardId}", id);
            return NotFound(new { Message = $"Credit card with Id {id} not found" });
        }

        return Ok(new { Message = "Default card updated successfully" });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCreditCard(Guid id)
    {
        var userId = GetUserId();
        _logger.LogInformation("⚡ DELETE Credit Card: {CardId} for user: {UserId}", id, userId);

        var command = new DeleteCreditCardCommand { Id = id, UserId = userId };
        var result = await _mediator.Send(command);

        if (!result)
        {
            _logger.LogWarning("⚠️ Failed to delete card: {CardId}", id);
            return NotFound(new { Message = $"Credit card with Id {id} not found" });
        }

        return Ok(new { Message = "Credit card deleted successfully" });
    }
}
