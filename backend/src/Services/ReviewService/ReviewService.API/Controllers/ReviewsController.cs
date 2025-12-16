using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ReviewService.Application.DTOs;
using ReviewService.Application.Features.Reviews.Commands.AdminDeleteReview;
using ReviewService.Application.Features.Reviews.Commands.CreateReview;
using ReviewService.Application.Features.Reviews.Commands.DeleteReview;
using ReviewService.Application.Features.Reviews.Commands.UpdateReview;
using ReviewService.Application.Features.Reviews.Queries.GetAllReviews;
using ReviewService.Application.Features.Reviews.Queries.GetReviewsByProduct;
using ReviewService.Application.Features.Reviews.Queries.GetReviewsByUser;
using System.Security.Claims;

namespace ReviewService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReviewsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ReviewsController> _logger;

    public ReviewsController(IMediator mediator, ILogger<ReviewsController> _logger)
    {
        _mediator = mediator;
        this._logger = _logger;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }

    [HttpPost]
    public async Task<ActionResult<ReviewDto>> CreateReview([FromBody] CreateReviewDto dto)
    {
        var userId = GetUserId();
        var command = new CreateReviewCommand(dto.ProductId, userId, dto.Comment, dto.Rating, dto.ImageUrls);
        var result = await _mediator.Send(command);

        _logger.LogInformation("✅ Review created for product {ProductId} by user {UserId}", dto.ProductId, userId);
        return CreatedAtAction(nameof(GetReviewsByProduct), new { productId = dto.ProductId }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReview(string id, [FromBody] UpdateReviewDto dto)
    {
        var userId = GetUserId();
        var command = new UpdateReviewCommand(id, userId, dto.Comment, dto.Rating, dto.ImageUrls);
        var result = await _mediator.Send(command);

        if (!result)
        {
            _logger.LogWarning("❌ Review {Id} not found or unauthorized for user {UserId}", id, userId);
            return NotFound(new { message = "Review not found or unauthorized" });
        }

        _logger.LogInformation("✅ Review {Id} updated by user {UserId}", id, userId);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReview(string id)
    {
        var userId = GetUserId();
        var command = new DeleteReviewCommand(id, userId);
        var result = await _mediator.Send(command);

        if (!result)
        {
            _logger.LogWarning("❌ Review {Id} not found or unauthorized for user {UserId}", id, userId);
            return NotFound(new { message = "Review not found or unauthorized" });
        }

        _logger.LogInformation("✅ Review {Id} deleted by user {UserId}", id, userId);
        return NoContent();
    }

    [HttpGet("product/{productId}")]
    [AllowAnonymous]
    public async Task<ActionResult<List<ReviewDto>>> GetReviewsByProduct(Guid productId)
    {
        var query = new GetReviewsByProductQuery(productId);
        var result = await _mediator.Send(query);

        _logger.LogInformation("📋 Retrieved {Count} reviews for product {ProductId}", result.Count, productId);
        return Ok(result);
    }

    [HttpGet("user")]
    public async Task<ActionResult<List<ReviewDto>>> GetMyReviews()
    {
        var userId = GetUserId();
        var query = new GetReviewsByUserQuery(userId);
        var result = await _mediator.Send(query);

        _logger.LogInformation("📋 Retrieved {Count} reviews for user {UserId}", result.Count, userId);
        return Ok(result);
    }

    [HttpGet("admin/all")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<List<ReviewDto>>> GetAllReviews()
    {
        var query = new GetAllReviewsQuery();
        var result = await _mediator.Send(query);

        _logger.LogInformation("📋 Admin retrieved all {Count} reviews", result.Count);
        return Ok(result);
    }

    [HttpDelete("admin/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AdminDeleteReview(string id)
    {
        var command = new AdminDeleteReviewCommand(id);
        var result = await _mediator.Send(command);

        if (!result)
        {
            _logger.LogWarning("❌ Admin: Review {Id} not found", id);
            return NotFound(new { message = "Review not found" });
        }

        _logger.LogInformation("✅ Admin deleted review {Id}", id);
        return NoContent();
    }
}
