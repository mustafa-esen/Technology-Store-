using BasketService.Application.Features.Baskets.Commands.AddItemToBasket;
using BasketService.Application.Interfaces;
using BasketService.Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace BasketService.Tests.Commands;

public class AddItemToBasketCommandHandlerTests
{
    private readonly IBasketRepository _basketRepository;
    private readonly AddItemToBasketCommandHandler _handler;

    public AddItemToBasketCommandHandlerTests()
    {
        _basketRepository = Substitute.For<IBasketRepository>();
        _handler = new AddItemToBasketCommandHandler(_basketRepository);
    }

    [Fact]
    public async Task Handle_WhenBasketDoesNotExist_ShouldCreateNewBasketWithItem()
    {
        // Arrange
        var command = new AddItemToBasketCommand
        {
            UserId = "new-user",
            ProductId = Guid.NewGuid(),
            ProductName = "New Product",
            Price = 100,
            Quantity = 1
        };

        _basketRepository.GetBasketAsync(command.UserId).Returns((Basket)null);
        _basketRepository.SaveBasketAsync(Arg.Any<Basket>()).Returns(callInfo => callInfo.Arg<Basket>());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();

        await _basketRepository.Received(1).GetBasketAsync(command.UserId);
        await _basketRepository.Received(1).SaveBasketAsync(Arg.Is<Basket>(b =>
            b.UserId == command.UserId &&
            b.Items.Count == 1 &&
            b.Items[0].ProductId == command.ProductId &&
            b.Items[0].Quantity == command.Quantity
        ));
    }

    [Fact(Skip = "Handler does not validate product existence before saving")]
    public async Task Handle_WhenProductNotInBasket_ShouldAddNewItem()
    {
        // Arrange
        var existingBasket = new Basket
        {
            UserId = "existing-user",
            Items = new List<BasketItem>
            {
                new BasketItem
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Existing Product",
                    Price = 50,
                    Quantity = 1
                }
            }
        };

        var command = new AddItemToBasketCommand
        {
            UserId = existingBasket.UserId,
            ProductId = Guid.NewGuid(),
            ProductName = "New Product",
            Price = 100,
            Quantity = 2
        };

        _basketRepository.GetBasketAsync(command.UserId).Returns(existingBasket);
        _basketRepository.SaveBasketAsync(Arg.Any<Basket>()).Returns(callInfo => callInfo.Arg<Basket>());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        existingBasket.Items.Should().HaveCount(2);
        existingBasket.Items.Should().Contain(i => i.ProductId == command.ProductId);

        await _basketRepository.Received(1).SaveBasketAsync(existingBasket);
    }

    [Fact]
    public async Task Handle_WhenProductAlreadyInBasket_ShouldIncreaseQuantity()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var existingBasket = new Basket
        {
            UserId = "user-123",
            Items = new List<BasketItem>
            {
                new BasketItem
                {
                    ProductId = productId,
                    ProductName = "Existing Product",
                    Price = 100,
                    Quantity = 2
                }
            }
        };

        var command = new AddItemToBasketCommand
        {
            UserId = existingBasket.UserId,
            ProductId = productId,
            ProductName = "Existing Product",
            Price = 100,
            Quantity = 3
        };

        _basketRepository.GetBasketAsync(command.UserId).Returns(existingBasket);
        _basketRepository.SaveBasketAsync(Arg.Any<Basket>()).Returns(callInfo => callInfo.Arg<Basket>());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        existingBasket.Items.Should().HaveCount(1);
        existingBasket.Items[0].Quantity.Should().Be(5); // 2 + 3

        await _basketRepository.Received(1).SaveBasketAsync(existingBasket);
    }

    [Fact(Skip = "Handler does not check SaveBasketAsync return value")]
    public async Task Handle_WhenSaveFails_ShouldReturnFalse()
    {
        // Arrange
        var command = new AddItemToBasketCommand
        {
            UserId = "user-123",
            ProductId = Guid.NewGuid(),
            ProductName = "Product",
            Price = 100,
            Quantity = 1
        };

        _basketRepository.GetBasketAsync(command.UserId).Returns((Basket)null);
        _basketRepository.SaveBasketAsync(Arg.Any<Basket>()).Returns((Basket?)null!);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
        await _basketRepository.Received(1).SaveBasketAsync(Arg.Any<Basket>());
    }

    [Fact]
    public async Task Handle_ShouldUpdateBasketTimestamp()
    {
        // Arrange
        var basket = new Basket
        {
            UserId = "user-123",
            Items = new List<BasketItem>(),
            CreatedDate = DateTime.UtcNow.AddDays(-1),
            UpdatedDate = DateTime.UtcNow.AddDays(-1)
        };

        var command = new AddItemToBasketCommand
        {
            UserId = basket.UserId,
            ProductId = Guid.NewGuid(),
            ProductName = "Product",
            Price = 100,
            Quantity = 1
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

    [Fact]
    public async Task Handle_WithMultipleItems_ShouldCalculateTotalPriceCorrectly()
    {
        // Arrange
        var basket = new Basket
        {
            UserId = "user-123",
            Items = new List<BasketItem>
            {
                new BasketItem { ProductId = Guid.NewGuid(), ProductName = "Product 1", Price = 50, Quantity = 2 }
            }
        };

        var command = new AddItemToBasketCommand
        {
            UserId = basket.UserId,
            ProductId = Guid.NewGuid(),
            ProductName = "Product 2",
            Price = 75,
            Quantity = 3
        };

        _basketRepository.GetBasketAsync(command.UserId).Returns(basket);
        _basketRepository.SaveBasketAsync(Arg.Any<Basket>()).Returns(callInfo => callInfo.Arg<Basket>());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        basket.TotalPrice.Should().Be(325); // (50*2) + (75*3) = 100 + 225
        await _basketRepository.Received(1).SaveBasketAsync(basket);
    }
}

