using MediatR;
using ReviewService.Application.DTOs;

namespace ReviewService.Application.Features.Reviews.Queries.GetReviewsByUser;

public record GetReviewsByUserQuery(Guid UserId) : IRequest<List<ReviewDto>>;
