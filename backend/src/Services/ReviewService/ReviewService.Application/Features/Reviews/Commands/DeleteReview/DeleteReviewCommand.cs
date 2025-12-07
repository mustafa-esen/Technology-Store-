using MediatR;

namespace ReviewService.Application.Features.Reviews.Commands.DeleteReview;

public record DeleteReviewCommand(
    string Id,
    Guid UserId
) : IRequest<bool>;
