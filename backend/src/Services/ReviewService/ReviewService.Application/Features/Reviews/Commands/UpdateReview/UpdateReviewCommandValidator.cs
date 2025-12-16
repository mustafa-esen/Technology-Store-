using FluentValidation;

namespace ReviewService.Application.Features.Reviews.Commands.UpdateReview;

public class UpdateReviewCommandValidator : AbstractValidator<UpdateReviewCommand>
{
    public UpdateReviewCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Review Id is required");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required");

        RuleFor(x => x.Comment)
            .NotEmpty().WithMessage("Comment is required")
            .MaximumLength(1000).WithMessage("Comment cannot exceed 1000 characters");

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5");
    }
}
