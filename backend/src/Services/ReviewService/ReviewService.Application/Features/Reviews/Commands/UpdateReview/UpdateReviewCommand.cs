using MediatR;

namespace ReviewService.Application.Features.Reviews.Commands.UpdateReview;

public record UpdateReviewCommand(
    string Id,
    Guid UserId,
    string Comment,
    int Rating,
    List<string>? ImageUrls
) : IRequest<bool>;
