using BasketService.Application.Features.Baskets.Commands.ClearBasket;
using BasketService.Application.Interfaces;
using FluentAssertions;
using NSubstitute;

namespace BasketService.Tests.Commands;

public class ClearBasketCommandHandlerTests
{
    private readonly IBasketRepository _basketRepository;
    private readonly ClearBasketCommandHandler _handler;

    public ClearBasketCommandHandlerTests()
    {
        _basketRepository = Substitute.For<IBasketRepository>();
        _handler = new ClearBasketCommandHandler(_basketRepository);
    }

    [Fact]
    public async Task Handle_WithValidUserId_ShouldDeleteBasketAndReturnTrue()
    {
        // Arrange
        var command = new ClearBasketCommand { UserId = "user-123" };
        _basketRepository.DeleteBasketAsync(command.UserId).Returns(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        await _basketRepository.Received(1).DeleteBasketAsync(command.UserId);
    }

    [Fact]
    public async Task Handle_WhenDeleteFails_ShouldReturnFalse()
    {
        // Arrange
        var command = new ClearBasketCommand { UserId = "user-123" };
        _basketRepository.DeleteBasketAsync(command.UserId).Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
        await _basketRepository.Received(1).DeleteBasketAsync(command.UserId);
    }

    [Fact]
    public async Task Handle_WithNonExistentUser_ShouldReturnFalse()
    {
        // Arrange
        var command = new ClearBasketCommand { UserId = "non-existent-user" };
        _basketRepository.DeleteBasketAsync(command.UserId).Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
        await _basketRepository.Received(1).DeleteBasketAsync(command.UserId);
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryOnce()
    {
        // Arrange
        var userId = "test-user";
        var command = new ClearBasketCommand { UserId = userId };
        _basketRepository.DeleteBasketAsync(userId).Returns(true);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _basketRepository.Received(1).DeleteBasketAsync(Arg.Is<string>(u => u == userId));
    }

    [Fact]
    public async Task Handle_WithEmptyUserId_ShouldStillCallRepository()
    {
        // Arrange
        var command = new ClearBasketCommand { UserId = string.Empty };
        _basketRepository.DeleteBasketAsync(string.Empty).Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
        await _basketRepository.Received(1).DeleteBasketAsync(string.Empty);
    }

    [Fact]
    public async Task Handle_MultipleCallsWithSameUser_ShouldCallRepositoryEachTime()
    {
        // Arrange
        var userId = "user-123";
        var command = new ClearBasketCommand { UserId = userId };
        _basketRepository.DeleteBasketAsync(userId).Returns(true);

        // Act
        await _handler.Handle(command, CancellationToken.None);
        await _handler.Handle(command, CancellationToken.None);
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _basketRepository.Received(3).DeleteBasketAsync(userId);
    }

    [Fact]
    public async Task Handle_WithDifferentUsers_ShouldCallRepositoryForEach()
    {
        // Arrange
        var command1 = new ClearBasketCommand { UserId = "user-1" };
        var command2 = new ClearBasketCommand { UserId = "user-2" };
        var command3 = new ClearBasketCommand { UserId = "user-3" };

        _basketRepository.DeleteBasketAsync(Arg.Any<string>()).Returns(true);

        // Act
        await _handler.Handle(command1, CancellationToken.None);
        await _handler.Handle(command2, CancellationToken.None);
        await _handler.Handle(command3, CancellationToken.None);

        // Assert
        await _basketRepository.Received(1).DeleteBasketAsync("user-1");
        await _basketRepository.Received(1).DeleteBasketAsync("user-2");
        await _basketRepository.Received(1).DeleteBasketAsync("user-3");
    }
}

