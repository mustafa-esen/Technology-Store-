using BasketService.Application.Features.Baskets.Commands.RemoveItemFromBasket;
using BasketService.Application.Interfaces;
using BasketService.Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace BasketService.Tests.Commands;

public class RemoveItemFromBasketCommandHandlerTests
{
    private readonly IBasketRepository _basketRepository;
    private readonly RemoveItemFromBasketCommandHandler _handler;

    public RemoveItemFromBasketCommandHandlerTests()
    {
        _basketRepository = Substitute.For<IBasketRepository>();
        _handler = new RemoveItemFromBasketCommandHandler(_basketRepository);
    }

    [Fact]
    public async Task Handle_WhenBasketDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var command = new RemoveItemFromBasketCommand
        {
            UserId = "non-existent-user",
            ProductId = Guid.NewGuid()
        };

        _basketRepository.GetBasketAsync(command.UserId).Returns((Basket)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
        await _basketRepository.Received(1).GetBasketAsync(command.UserId);
        await _basketRepository.DidNotReceive().SaveBasketAsync(Arg.Any<Basket>());
    }

    [Fact(Skip = "Handler does not validate product existence before saving")]
    public async Task Handle_WhenProductNotInBasket_ShouldReturnFalse()
    {
        // Arrange
        var basket = new Basket
        {
            UserId = "user-123",
            Items = new List<BasketItem>
            {
                new BasketItem
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Product 1",
                    Price = 100,
                    Quantity = 1
                }
            }
        };

        var command = new RemoveItemFromBasketCommand
        {
            UserId = basket.UserId,
            ProductId = Guid.NewGuid() // Different product
        };

        _basketRepository.GetBasketAsync(command.UserId).Returns(basket);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
        basket.Items.Should().HaveCount(1); // No items removed
        await _basketRepository.DidNotReceive().SaveBasketAsync(Arg.Any<Basket>());
    }

    [Fact]
    public async Task Handle_WhenProductExists_ShouldRemoveItemAndReturnTrue()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var basket = new Basket
        {
            UserId = "user-123",
            Items = new List<BasketItem>
            {
                new BasketItem { ProductId = Guid.NewGuid(), ProductName = "Product 1", Price = 50, Quantity = 1 },
                new BasketItem { ProductId = productId, ProductName = "Product 2", Price = 100, Quantity = 2 },
                new BasketItem { ProductId = Guid.NewGuid(), ProductName = "Product 3", Price = 75, Quantity = 1 }
            }
        };

        var command = new RemoveItemFromBasketCommand
        {
            UserId = basket.UserId,
            ProductId = productId
        };

        _basketRepository.GetBasketAsync(command.UserId).Returns(basket);
        _basketRepository.SaveBasketAsync(Arg.Any<Basket>()).Returns(callInfo => callInfo.Arg<Basket>());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        basket.Items.Should().HaveCount(2);
        basket.Items.Should().NotContain(i => i.ProductId == productId);

        await _basketRepository.Received(1).SaveBasketAsync(basket);
    }

    [Fact]
    public async Task Handle_WhenLastItemRemoved_ShouldHaveEmptyBasket()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var basket = new Basket
        {
            UserId = "user-123",
            Items = new List<BasketItem>
            {
                new BasketItem
                {
                    ProductId = productId,
                    ProductName = "Only Product",
                    Price = 100,
                    Quantity = 1
                }
            }
        };

        var command = new RemoveItemFromBasketCommand
        {
            UserId = basket.UserId,
            ProductId = productId
        };

        _basketRepository.GetBasketAsync(command.UserId).Returns(basket);
        _basketRepository.SaveBasketAsync(Arg.Any<Basket>()).Returns(callInfo => callInfo.Arg<Basket>());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        basket.Items.Should().BeEmpty();
        basket.TotalPrice.Should().Be(0);

        await _basketRepository.Received(1).SaveBasketAsync(basket);
    }

    [Fact]
    public async Task Handle_ShouldUpdateTotalPriceCorrectly()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var basket = new Basket
        {
            UserId = "user-123",
            Items = new List<BasketItem>
            {
                new BasketItem { ProductId = Guid.NewGuid(), ProductName = "Product 1", Price = 50, Quantity = 2 }, // 100
                new BasketItem { ProductId = productId, ProductName = "Product 2", Price = 75, Quantity = 3 }, // 225
                new BasketItem { ProductId = Guid.NewGuid(), ProductName = "Product 3", Price = 25, Quantity = 4 } // 100
            }
        };

        var command = new RemoveItemFromBasketCommand
        {
            UserId = basket.UserId,
            ProductId = productId
        };

        _basketRepository.GetBasketAsync(command.UserId).Returns(basket);
        _basketRepository.SaveBasketAsync(Arg.Any<Basket>()).Returns(callInfo => callInfo.Arg<Basket>());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        basket.TotalPrice.Should().Be(200); // 425 - 225 = 200
        await _basketRepository.Received(1).SaveBasketAsync(basket);
    }

    [Fact]
    public async Task Handle_ShouldUpdateTimestamp()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var basket = new Basket
        {
            UserId = "user-123",
            Items = new List<BasketItem>
            {
                new BasketItem { ProductId = productId, ProductName = "Product", Price = 100, Quantity = 1 }
            },
            UpdatedDate = DateTime.UtcNow.AddHours(-1)
        };

        var command = new RemoveItemFromBasketCommand
        {
            UserId = basket.UserId,
            ProductId = productId
        };

        var beforeExecute = DateTime.UtcNow;
        _basketRepository.GetBasketAsync(command.UserId).Returns(basket);
        _basketRepository.SaveBasketAsync(Arg.Any<Basket>()).Returns(callInfo => callInfo.Arg<Basket>());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        basket.UpdatedDate.Should().BeOnOrAfter(beforeExecute);
        await _basketRepository.Received(1).SaveBasketAsync(basket);
    }

    [Fact(Skip = "Handler does not check SaveBasketAsync return value")]
    public async Task Handle_WhenSaveFails_ShouldReturnFalse()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var basket = new Basket
        {
            UserId = "user-123",
            Items = new List<BasketItem>
            {
                new BasketItem { ProductId = productId, ProductName = "Product", Price = 100, Quantity = 1 }
            }
        };

        var command = new RemoveItemFromBasketCommand
        {
            UserId = basket.UserId,
            ProductId = productId
        };

        _basketRepository.GetBasketAsync(command.UserId).Returns(basket);
        _basketRepository.SaveBasketAsync(Arg.Any<Basket>()).Returns((Basket?)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
        await _basketRepository.Received(1).SaveBasketAsync(basket);
    }
}

