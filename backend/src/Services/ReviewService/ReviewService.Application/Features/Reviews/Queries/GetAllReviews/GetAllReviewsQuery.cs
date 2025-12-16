using MediatR;
using ReviewService.Application.DTOs;

namespace ReviewService.Application.Features.Reviews.Queries.GetAllReviews;

public record GetAllReviewsQuery : IRequest<List<ReviewDto>>;
