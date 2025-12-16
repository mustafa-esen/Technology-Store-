using AutoMapper;
using MediatR;
using ReviewService.Application.DTOs;
using ReviewService.Application.Interfaces;

namespace ReviewService.Application.Features.Reviews.Queries.GetReviewsByUser;

public class GetReviewsByUserQueryHandler : IRequestHandler<GetReviewsByUserQuery, List<ReviewDto>>
{
    private readonly IReviewRepository _repository;
    private readonly IMapper _mapper;

    public GetReviewsByUserQueryHandler(IReviewRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<ReviewDto>> Handle(GetReviewsByUserQuery request, CancellationToken cancellationToken)
    {
        var reviews = await _repository.GetByUserIdAsync(request.UserId);
        return _mapper.Map<List<ReviewDto>>(reviews);
    }
}
