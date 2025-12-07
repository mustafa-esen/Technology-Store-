using MediatR;
using ReviewService.Application.Interfaces;

namespace ReviewService.Application.Features.Reviews.Commands.UpdateReview;

public class UpdateReviewCommandHandler : IRequestHandler<UpdateReviewCommand, bool>
{
    private readonly IReviewRepository _repository;

    public UpdateReviewCommandHandler(IReviewRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
    {
        var existingReview = await _repository.GetByIdAsync(request.Id);
        if (existingReview == null || existingReview.UserId != request.UserId)
            return false;

        existingReview.Comment = request.Comment;
        existingReview.Rating = request.Rating;
        existingReview.ImageUrls = request.ImageUrls ?? new List<string>();
        existingReview.UpdatedAt = DateTime.UtcNow;

        return await _repository.UpdateAsync(request.Id, existingReview);
    }
}
