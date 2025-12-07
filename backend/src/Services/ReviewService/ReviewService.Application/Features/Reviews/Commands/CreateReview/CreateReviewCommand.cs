using MediatR;
using ReviewService.Application.DTOs;

namespace ReviewService.Application.Features.Reviews.Commands.CreateReview;

public record CreateReviewCommand(
    Guid ProductId,
    Guid UserId,
    string Comment,
    int Rating,
    List<string>? ImageUrls
) : IRequest<ReviewDto>;
