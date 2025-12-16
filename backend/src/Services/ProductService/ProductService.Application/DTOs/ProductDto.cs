namespace ProductService.Application.DTOs;

public record ProductDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    int Stock,
    Guid CategoryId,
    string CategoryName,
    string Brand,
    string ImageUrl,
    bool IsActive
);

public record CreateProductDto(
    string Name,
    string Description,
    decimal Price,
    int Stock,
    Guid CategoryId,
    string Brand,
    string ImageUrl
);

public record UpdateProductDto(
    string Name,
    string Description,
    decimal Price,
    int Stock,
    Guid CategoryId,
    string Brand,
    string ImageUrl,
    bool IsActive
);
