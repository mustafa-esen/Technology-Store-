using MediatR;
using ReviewService.Application.Interfaces;

namespace ReviewService.Application.Features.Reviews.Commands.DeleteReview;

public class DeleteReviewCommandHandler : IRequestHandler<DeleteReviewCommand, bool>
{
    private readonly IReviewRepository _repository;

    public DeleteReviewCommandHandler(IReviewRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
    {
        var existingReview = await _repository.GetByIdAsync(request.Id);
        if (existingReview == null || existingReview.UserId != request.UserId)
            return false;

        return await _repository.DeleteAsync(request.Id);
    }
}
