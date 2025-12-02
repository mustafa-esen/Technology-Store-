using MediatR;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Application.Features.Payments.Queries.GetPaymentById;
using PaymentService.Application.Features.Payments.Queries.GetPaymentsByUserId;

namespace PaymentService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(IMediator mediator, ILogger<PaymentsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPaymentById(Guid id)
    {
        _logger.LogInformation("⚡ GET Payment by Id: {PaymentId}", id);

        var query = new GetPaymentByIdQuery { Id = id };
        var result = await _mediator.Send(query);

        if (result == null)
        {
            _logger.LogWarning("⚠️ Payment not found: {PaymentId}", id);
            return NotFound(new { Message = $"Payment with Id {id} not found" });
        }

        return Ok(result);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetPaymentsByUserId(string userId)
    {
        _logger.LogInformation("⚡ GET Payments for UserId: {UserId}", userId);

        var query = new GetPaymentsByUserIdQuery { UserId = userId };
        var result = await _mediator.Send(query);

        return Ok(result);
    }
}
