using FluentAssertions;
using Moq;
using ProductService.Application.Features.Products.Commands.UpdateProduct;
using ProductService.Application.Interfaces;
using ProductService.Domain.Entities;

namespace ProductService.Tests.Commands;

public class UpdateProductCommandHandlerTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly UpdateProductCommandHandler _handler;

    public UpdateProductCommandHandlerTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _handler = new UpdateProductCommandHandler(
            _productRepositoryMock.Object,
            _categoryRepositoryMock.Object
        );
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldUpdateProduct()
    {
        var productId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var existingProduct = new Product
        {
            Id = productId,
            Name = "Old Name",
            Description = "Old Description",
            Price = 100m,
            Stock = 10,
            CategoryId = Guid.NewGuid(),
            Brand = "Old Brand",
            ImageUrl = "old.jpg",
            IsActive = true
        };

        var command = new UpdateProductCommand(
            productId,
            "Updated Name",
            "Updated Description",
            150m,
            20,
            categoryId,
            "New Brand",
            "new.jpg",
            false
        );

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync(existingProduct);

        _categoryRepositoryMock
            .Setup(x => x.ExistsAsync(categoryId))
            .ReturnsAsync(true);

        _productRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Product>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        existingProduct.Name.Should().Be("Updated Name");
        existingProduct.Description.Should().Be("Updated Description");
        existingProduct.Price.Should().Be(150m);
        existingProduct.Stock.Should().Be(20);
        existingProduct.CategoryId.Should().Be(categoryId);
        existingProduct.Brand.Should().Be("New Brand");
        existingProduct.ImageUrl.Should().Be("new.jpg");
        existingProduct.IsActive.Should().BeFalse();
        existingProduct.UpdatedAt.Should().NotBeNull();

        _productRepositoryMock.Verify(x => x.UpdateAsync(existingProduct), Times.Once);
    }

    [Fact]
    public async Task Handle_ProductNotFound_ShouldReturnFalse()
    {
        var productId = Guid.NewGuid();
        var command = new UpdateProductCommand(
            productId,
            "Name",
            "Description",
            100m,
            10,
            Guid.NewGuid(),
            "Brand",
            "image.jpg",
            true
        );

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync((Product?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
        _productRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task Handle_InvalidCategoryId_ShouldThrowArgumentException()
    {
        var productId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var existingProduct = new Product { Id = productId };

        var command = new UpdateProductCommand(
            productId,
            "Name",
            "Description",
            100m,
            10,
            categoryId,
            "Brand",
            "image.jpg",
            true
        );

        _productRepositoryMock
            .Setup(x => x.GetByIdAsync(productId))
            .ReturnsAsync(existingProduct);

        _categoryRepositoryMock
            .Setup(x => x.ExistsAsync(categoryId))
            .ReturnsAsync(false);

        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage($"Category with ID {categoryId} not found");
    }
}
