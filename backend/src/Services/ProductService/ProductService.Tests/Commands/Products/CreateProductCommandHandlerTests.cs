using AutoMapper;
using FluentAssertions;
using Moq;
using ProductService.Application.DTOs;
using ProductService.Application.Features.Products.Commands.CreateProduct;
using ProductService.Application.Interfaces;
using ProductService.Domain.Entities;

namespace ProductService.Tests.Commands;

public class CreateProductCommandHandlerTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new CreateProductCommandHandler(
            _productRepositoryMock.Object,
            _categoryRepositoryMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldCreateProduct()
    {
        var categoryId = Guid.NewGuid();
        var command = new CreateProductCommand(
            "iPhone 15 Pro",
            "Latest Apple smartphone",
            999.99m,
            50,
            categoryId,
            "Apple",
            "https://example.com/iphone.jpg"
        );

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Description = command.Description,
            Price = command.Price,
            Stock = command.Stock,
            CategoryId = command.CategoryId,
            Brand = command.Brand,
            ImageUrl = command.ImageUrl,
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

        _categoryRepositoryMock
            .Setup(x => x.ExistsAsync(categoryId))
            .ReturnsAsync(true);

        _mapperMock
            .Setup(x => x.Map<Product>(command))
            .Returns(product);

        _productRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Product>()))
            .ReturnsAsync(product);

        _mapperMock
            .Setup(x => x.Map<ProductDto>(product))
            .Returns(productDto);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Name.Should().Be("iPhone 15 Pro");
        result.Price.Should().Be(999.99m);
        result.Stock.Should().Be(50);
        result.Brand.Should().Be("Apple");
        result.CategoryName.Should().Be("Smartphones");
        
        _categoryRepositoryMock.Verify(x => x.ExistsAsync(categoryId), Times.Once);
        _productRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidCategoryId_ShouldThrowArgumentException()
    {
        var categoryId = Guid.NewGuid();
        var command = new CreateProductCommand(
            "iPhone 15 Pro",
            "Latest Apple smartphone",
            999.99m,
            50,
            categoryId,
            "Apple",
            "https://example.com/iphone.jpg"
        );

        _categoryRepositoryMock
            .Setup(x => x.ExistsAsync(categoryId))
            .ReturnsAsync(false);

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage($"Category with ID {categoryId} not found");

        _productRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ProductWithZeroStock_ShouldCreateSuccessfully()
    {
        var categoryId = Guid.NewGuid();
        var command = new CreateProductCommand(
            "Out of Stock Product",
            "This product is currently unavailable",
            100m,
            0,
            categoryId,
            "TestBrand",
            "https://example.com/image.jpg"
        );

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Stock = 0,
            Category = new Category { Id = categoryId, Name = "Test" }
        };

        var productDto = new ProductDto(
            product.Id,
            command.Name,
            command.Description,
            command.Price,
            0,
            categoryId,
            "Test",
            command.Brand,
            command.ImageUrl,
            true
        );

        _categoryRepositoryMock.Setup(x => x.ExistsAsync(categoryId)).ReturnsAsync(true);
        _mapperMock.Setup(x => x.Map<Product>(command)).Returns(product);
        _productRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Product>())).ReturnsAsync(product);
        _mapperMock.Setup(x => x.Map<ProductDto>(product)).Returns(productDto);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().NotBeNull();
        result.Stock.Should().Be(0);
    }
}
