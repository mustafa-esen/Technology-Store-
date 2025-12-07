using AutoMapper;
using MediatR;
using ReviewService.Application.DTOs;
using ReviewService.Application.Interfaces;

namespace ReviewService.Application.Features.Reviews.Queries.GetReviewsByProduct;

public class GetReviewsByProductQueryHandler : IRequestHandler<GetReviewsByProductQuery, List<ReviewDto>>
{
    private readonly IReviewRepository _repository;
    private readonly IMapper _mapper;

    public GetReviewsByProductQueryHandler(IReviewRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<ReviewDto>> Handle(GetReviewsByProductQuery request, CancellationToken cancellationToken)
    {
        var reviews = await _repository.GetByProductIdAsync(request.ProductId);
        return _mapper.Map<List<ReviewDto>>(reviews);
    }
}
