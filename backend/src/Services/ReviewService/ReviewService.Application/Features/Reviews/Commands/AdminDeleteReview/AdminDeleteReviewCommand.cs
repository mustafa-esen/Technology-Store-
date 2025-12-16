using MediatR;

namespace ReviewService.Application.Features.Reviews.Commands.AdminDeleteReview;

public record AdminDeleteReviewCommand(string ReviewId) : IRequest<bool>;
