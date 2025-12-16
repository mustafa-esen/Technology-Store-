using AutoMapper;
using MediatR;
using ReviewService.Application.DTOs;
using ReviewService.Application.Interfaces;

namespace ReviewService.Application.Features.Reviews.Queries.GetAllReviews;

public class GetAllReviewsQueryHandler : IRequestHandler<GetAllReviewsQuery, List<ReviewDto>>
{
    private readonly IReviewRepository _repository;
    private readonly IMapper _mapper;

    public GetAllReviewsQueryHandler(IReviewRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<ReviewDto>> Handle(GetAllReviewsQuery request, CancellationToken cancellationToken)
    {
        var reviews = await _repository.GetAllAsync();
        return _mapper.Map<List<ReviewDto>>(reviews);
    }
}
