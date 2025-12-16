using FluentAssertions;
using Moq;
using ProductService.Application.Features.Products.Commands.DeleteProduct;
using ProductService.Application.Interfaces;

namespace ProductService.Tests.Commands;

public class DeleteProductCommandHandlerTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly DeleteProductCommandHandler _handler;

    public DeleteProductCommandHandlerTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _handler = new DeleteProductCommandHandler(_productRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingProduct_ShouldDeleteAndReturnTrue()
    {
        var productId = Guid.NewGuid();
        var command = new DeleteProductCommand(productId);

        _productRepositoryMock
            .Setup(x => x.ExistsAsync(productId))
            .ReturnsAsync(true);

        _productRepositoryMock
            .Setup(x => x.DeleteAsync(productId))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeTrue();
        _productRepositoryMock.Verify(x => x.DeleteAsync(productId), Times.Once);
    }

    [Fact]
    public async Task Handle_NonExistingProduct_ShouldReturnFalse()
    {
        var productId = Guid.NewGuid();
        var command = new DeleteProductCommand(productId);

        _productRepositoryMock
            .Setup(x => x.ExistsAsync(productId))
            .ReturnsAsync(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Should().BeFalse();
        _productRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<Guid>()), Times.Never);
    }
}
