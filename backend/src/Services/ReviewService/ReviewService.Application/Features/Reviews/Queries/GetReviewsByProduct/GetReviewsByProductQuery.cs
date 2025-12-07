using MediatR;
using ReviewService.Application.DTOs;

namespace ReviewService.Application.Features.Reviews.Queries.GetReviewsByProduct;

public record GetReviewsByProductQuery(Guid ProductId) : IRequest<List<ReviewDto>>;
