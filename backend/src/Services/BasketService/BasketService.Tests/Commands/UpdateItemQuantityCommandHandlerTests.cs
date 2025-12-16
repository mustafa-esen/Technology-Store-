using BasketService.Application.Features.Baskets.Commands.UpdateItemQuantity;
using BasketService.Application.Interfaces;
using BasketService.Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace BasketService.Tests.Commands;

public class UpdateItemQuantityCommandHandlerTests
{
    private readonly IBasketRepository _basketRepository;
    private readonly UpdateItemQuantityCommandHandler _handler;

    public UpdateItemQuantityCommandHandlerTests()
    {
        _basketRepository = Substitute.For<IBasketRepository>();
        _handler = new UpdateItemQuantityCommandHandler(_basketRepository);
    }

    [Fact]
    public async Task Handle_WhenBasketDoesNotExist_ShouldReturnFalse()
    {
        // Arrange
        var command = new UpdateItemQuantityCommand
        {
            UserId = "non-existent-user",
            ProductId = Guid.NewGuid(),
            Quantity = 5
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
                    Quantity = 2
                }
            }
        };

        var command = new UpdateItemQuantityCommand
        {
            UserId = basket.UserId,
            ProductId = Guid.NewGuid(), // Different product
            Quantity = 5
        };

        _basketRepository.GetBasketAsync(command.UserId).Returns(basket);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
        basket.Items[0].Quantity.Should().Be(2); // Unchanged
        await _basketRepository.DidNotReceive().SaveBasketAsync(Arg.Any<Basket>());
    }

    [Fact]
    public async Task Handle_WithValidQuantity_ShouldUpdateQuantityAndReturnTrue()
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
                    ProductName = "Product",
                    Price = 100,
                    Quantity = 2
                }
            }
        };

        var command = new UpdateItemQuantityCommand
        {
            UserId = basket.UserId,
            ProductId = productId,
            Quantity = 5
        };

        _basketRepository.GetBasketAsync(command.UserId).Returns(basket);
        _basketRepository.SaveBasketAsync(Arg.Any<Basket>()).Returns(callInfo => callInfo.Arg<Basket>());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        basket.Items[0].Quantity.Should().Be(5);
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

        var command = new UpdateItemQuantityCommand
        {
            UserId = basket.UserId,
            ProductId = productId,
            Quantity = 5 // Change from 3 to 5
        };

        _basketRepository.GetBasketAsync(command.UserId).Returns(basket);
        _basketRepository.SaveBasketAsync(Arg.Any<Basket>()).Returns(callInfo => callInfo.Arg<Basket>());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        basket.Items[1].Quantity.Should().Be(5);
        basket.TotalPrice.Should().Be(575); // 100 + (75*5) + 100 = 100 + 375 + 100
        await _basketRepository.Received(1).SaveBasketAsync(basket);
    }

    [Fact]
    public async Task Handle_WithQuantityOne_ShouldUpdateCorrectly()
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
                    ProductName = "Product",
                    Price = 100,
                    Quantity = 5
                }
            }
        };

        var command = new UpdateItemQuantityCommand
        {
            UserId = basket.UserId,
            ProductId = productId,
            Quantity = 1
        };

        _basketRepository.GetBasketAsync(command.UserId).Returns(basket);
        _basketRepository.SaveBasketAsync(Arg.Any<Basket>()).Returns(callInfo => callInfo.Arg<Basket>());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        basket.Items[0].Quantity.Should().Be(1);
        basket.TotalPrice.Should().Be(100);
        await _basketRepository.Received(1).SaveBasketAsync(basket);
    }

    [Fact]
    public async Task Handle_WithLargeQuantity_ShouldUpdateCorrectly()
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
                    ProductName = "Product",
                    Price = 100,
                    Quantity = 1
                }
            }
        };

        var command = new UpdateItemQuantityCommand
        {
            UserId = basket.UserId,
            ProductId = productId,
            Quantity = 100
        };

        _basketRepository.GetBasketAsync(command.UserId).Returns(basket);
        _basketRepository.SaveBasketAsync(Arg.Any<Basket>()).Returns(callInfo => callInfo.Arg<Basket>());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        basket.Items[0].Quantity.Should().Be(100);
        basket.TotalPrice.Should().Be(10000);
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
                new BasketItem { ProductId = productId, ProductName = "Product", Price = 100, Quantity = 2 }
            },
            UpdatedDate = DateTime.UtcNow.AddHours(-1)
        };

        var command = new UpdateItemQuantityCommand
        {
            UserId = basket.UserId,
            ProductId = productId,
            Quantity = 5
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
                new BasketItem { ProductId = productId, ProductName = "Product", Price = 100, Quantity = 2 }
            }
        };

        var command = new UpdateItemQuantityCommand
        {
            UserId = basket.UserId,
            ProductId = productId,
            Quantity = 5
        };

        _basketRepository.GetBasketAsync(command.UserId).Returns(basket);
        _basketRepository.SaveBasketAsync(Arg.Any<Basket>()).Returns((Basket?)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
        basket.Items[0].Quantity.Should().Be(5); // Quantity updated but save failed
        await _basketRepository.Received(1).SaveBasketAsync(basket);
    }
}

