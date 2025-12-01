using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Features.Products.Queries.CheckStock;

namespace ProductService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StockController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<StockController> _logger;

    public StockController(IMediator mediator, ILogger<StockController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost("check")]
    public async Task<ActionResult<CheckStockResult>> CheckStock([FromBody] CheckStockQuery query)
    {
        _logger.LogInformation("👤 Request: POST /api/stock/check with {ItemCount} items", query.Items.Count);

        var result = await _mediator.Send(query);

        if (!result.IsAvailable)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
