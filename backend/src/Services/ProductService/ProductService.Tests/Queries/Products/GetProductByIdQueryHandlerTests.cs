using AutoMapper;
using FluentAssertions;
using Moq;
using ProductService.Application.DTOs;
using ProductService.Application.Features.Products.Queries.GetProductById;
using ProductService.Application.Interfaces;
using ProductService.Domain.Entities;

namespace ProductService.Tests.Queries;

public class GetProductByIdQueryHandlerTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetProductByIdQueryHandler _handler;

    public GetProductByIdQueryHandlerTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetProductByIdQueryHandler(_productRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ProductExists_ShouldReturnProductDto()
    {
        var productId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            Name = "iPhone 15",
            Description = "Latest iPhone",
            Price = 999m,
            Stock = 50,
            CategoryId = categoryId,
            Brand = "Apple",
            ImageUrl = "iphone.jpg",
            IsActive = true,
            Category = new Category { Id = categoryId, Name = "Smartphones" }
        };

        var productDto = new ProductDto(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.Stock,
            product.CategoryId,
            "Smartphones",
            product.Brand,
            product.ImageUrl,
            product.IsActive
        );

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync(product);

        _mapperMock
            .Setup(x => x.Map<ProductDto>(product))
            .Returns(productDto);

        var query = new GetProductByIdQuery(productId);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(productId);
        result.Name.Should().Be("iPhone 15");
        result.CategoryName.Should().Be("Smartphones");
    }

    [Fact]
    public async Task Handle_ProductNotFound_ShouldReturnNull()
    {
        var productId = Guid.NewGuid();

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync((Product?)null);

        var query = new GetProductByIdQuery(productId);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().BeNull();
        _mapperMock.Verify(x => x.Map<ProductDto>(It.IsAny<Product>()), Times.Never);
    }
}
