using AutoMapper;
using FluentAssertions;
using Moq;
using ProductService.Application.DTOs;
using ProductService.Application.Features.Products.Queries.GetAllProducts;
using ProductService.Application.Interfaces;
using ProductService.Domain.Entities;

namespace ProductService.Tests.Queries;

public class GetAllProductsQueryHandlerTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetAllProductsQueryHandler _handler;

    public GetAllProductsQueryHandlerTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetAllProductsQueryHandler(_productRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ProductsExist_ShouldReturnProductDtos()
    {
        var categoryId = Guid.NewGuid();
        var products = new List<Product>
        {
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "iPhone 15",
                Price = 999m,
                Stock = 50,
                CategoryId = categoryId,
                Brand = "Apple",
                IsActive = true,
                Category = new Category { Id = categoryId, Name = "Smartphones" }
            },
            new Product
            {
                Id = Guid.NewGuid(),
                Name = "MacBook Pro",
                Price = 2499m,
                Stock = 30,
                CategoryId = categoryId,
                Brand = "Apple",
                IsActive = true,
                Category = new Category { Id = categoryId, Name = "Laptops" }
            }
        };

        var productDtos = products.Select(p => new ProductDto(
            p.Id,
            p.Name,
            p.Description,
            p.Price,
            p.Stock,
            p.CategoryId,
            p.Category.Name,
            p.Brand,
            p.ImageUrl,
            p.IsActive
        )).ToList();

        _productRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(products);

        _mapperMock
            .Setup(x => x.Map<IEnumerable<ProductDto>>(products))
            .Returns(productDtos);

        var query = new GetAllProductsQuery();
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.First().Name.Should().Be("iPhone 15");
        result.Last().Name.Should().Be("MacBook Pro");
    }

    [Fact]
    public async Task Handle_NoProducts_ShouldReturnEmptyList()
    {
        _productRepositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(new List<Product>());

        _mapperMock
            .Setup(x => x.Map<IEnumerable<ProductDto>>(It.IsAny<List<Product>>()))
            .Returns(new List<ProductDto>());

        var query = new GetAllProductsQuery();
        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}
