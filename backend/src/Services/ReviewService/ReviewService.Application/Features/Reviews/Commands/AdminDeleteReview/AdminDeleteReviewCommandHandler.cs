using MediatR;
using ReviewService.Application.Interfaces;

namespace ReviewService.Application.Features.Reviews.Commands.AdminDeleteReview;

public class AdminDeleteReviewCommandHandler : IRequestHandler<AdminDeleteReviewCommand, bool>
{
    private readonly IReviewRepository _repository;

    public AdminDeleteReviewCommandHandler(IReviewRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(AdminDeleteReviewCommand request, CancellationToken cancellationToken)
    {
        return await _repository.DeleteAsync(request.ReviewId);
    }
}
