using AutoMapper;
using MediatR;
using ReviewService.Application.DTOs;
using ReviewService.Application.Interfaces;
using ReviewService.Domain.Entities;

namespace ReviewService.Application.Features.Reviews.Commands.CreateReview;

public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, ReviewDto>
{
    private readonly IReviewRepository _repository;
    private readonly IMapper _mapper;

    public CreateReviewCommandHandler(IReviewRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ReviewDto> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        var review = new ProductReview
        {
            ProductId = request.ProductId,
            UserId = request.UserId,
            Comment = request.Comment,
            Rating = request.Rating,
            ImageUrls = request.ImageUrls ?? new List<string>(),
            CreatedAt = DateTime.UtcNow
        };

        var createdReview = await _repository.CreateAsync(review);
        return _mapper.Map<ReviewDto>(createdReview);
    }
}
