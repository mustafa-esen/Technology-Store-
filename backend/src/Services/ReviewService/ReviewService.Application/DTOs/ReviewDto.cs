namespace ReviewService.Application.DTOs;

public record ReviewDto(
    string Id,
    Guid ProductId,
    Guid UserId,
    string Comment,
    int Rating,
    List<string> ImageUrls,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record CreateReviewDto(
    Guid ProductId,
    string Comment,
    int Rating,
    List<string>? ImageUrls
);

public record UpdateReviewDto(
    string Comment,
    int Rating,
    List<string>? ImageUrls
);
