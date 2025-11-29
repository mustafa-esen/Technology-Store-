using AutoMapper;
using BasketService.Application.DTOs;
using BasketService.Application.Features.Baskets.Queries.GetBasket;
using BasketService.Application.Interfaces;
using BasketService.Domain.Entities;
using FluentAssertions;
using NSubstitute;

namespace BasketService.Tests.Queries;

public class GetBasketQueryHandlerTests
{
    private readonly IBasketRepository _basketRepository;
    private readonly IMapper _mapper;
    private readonly GetBasketQueryHandler _handler;

    public GetBasketQueryHandlerTests()
    {
        _basketRepository = Substitute.For<IBasketRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetBasketQueryHandler(_basketRepository, _mapper);
    }

    [Fact]
    public async Task Handle_WithValidUserId_ShouldReturnBasket()
    {
        // Arrange
        var userId = "test-user-123";
        var basket = new Basket
        {
            UserId = userId,
            Items = new List<BasketItem>
            {
                new BasketItem
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Test Product",
                    Price = 100,
                    Quantity = 2
                }
            }
        };

        var basketDto = new BasketDto
        {
            UserId = userId,
            Items = new List<BasketItemDto>
            {
                new BasketItemDto
                {
                    ProductId = basket.Items[0].ProductId,
                    ProductName = "Test Product",
                    Price = 100,
                    Quantity = 2
                }
            },
            TotalPrice = 200
        };

        _basketRepository.GetBasketAsync(userId).Returns(basket);
        _mapper.Map<BasketDto>(basket).Returns(basketDto);

        var query = new GetBasketQuery { UserId = userId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.UserId.Should().Be(userId);
        result.Items.Should().HaveCount(1);
        result.TotalPrice.Should().Be(200);

        await _basketRepository.Received(1).GetBasketAsync(userId);
    }

    [Fact]
    public async Task Handle_WithNonExistentUser_ShouldReturnNull()
    {
        // Arrange
        var userId = "non-existent-user";
        _basketRepository.GetBasketAsync(userId).Returns((Basket?)null);

        var query = new GetBasketQuery { UserId = userId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
        await _basketRepository.Received(1).GetBasketAsync(userId);
    }

    [Fact]
    public async Task Handle_WithEmptyBasket_ShouldReturnBasketWithNoItems()
    {
        // Arrange
        var userId = "user-with-empty-basket";
        var basket = new Basket
        {
            UserId = userId,
            Items = new List<BasketItem>()
        };

        var basketDto = new BasketDto
        {
            UserId = userId,
            Items = new List<BasketItemDto>(),
            TotalPrice = 0
        };

        _basketRepository.GetBasketAsync(userId).Returns(basket);
        _mapper.Map<BasketDto>(basket).Returns(basketDto);

        var query = new GetBasketQuery { UserId = userId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.UserId.Should().Be(userId);
        result.Items.Should().BeEmpty();
        result.TotalPrice.Should().Be(0);
    }

    [Fact]
    public async Task Handle_WithMultipleItems_ShouldCalculateTotalPriceCorrectly()
    {
        // Arrange
        var userId = "user-multiple-items";
        var basket = new Basket
        {
            UserId = userId,
            Items = new List<BasketItem>
            {
                new BasketItem { ProductId = Guid.NewGuid(), ProductName = "Product 1", Price = 100, Quantity = 2 },
                new BasketItem { ProductId = Guid.NewGuid(), ProductName = "Product 2", Price = 50, Quantity = 3 },
                new BasketItem { ProductId = Guid.NewGuid(), ProductName = "Product 3", Price = 25, Quantity = 4 }
            }
        };

        var basketDto = new BasketDto
        {
            UserId = userId,
            Items = new List<BasketItemDto>
            {
                new BasketItemDto { ProductId = basket.Items[0].ProductId, ProductName = "Product 1", Price = 100, Quantity = 2 },
                new BasketItemDto { ProductId = basket.Items[1].ProductId, ProductName = "Product 2", Price = 50, Quantity = 3 },
                new BasketItemDto { ProductId = basket.Items[2].ProductId, ProductName = "Product 3", Price = 25, Quantity = 4 }
            },
            TotalPrice = 450
        };

        _basketRepository.GetBasketAsync(userId).Returns(basket);
        _mapper.Map<BasketDto>(basket).Returns(basketDto);

        var query = new GetBasketQuery { UserId = userId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Items.Should().HaveCount(3);
        result.TotalPrice.Should().Be(450); // (100*2) + (50*3) + (25*4) = 200 + 150 + 100
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryOnce()
    {
        // Arrange
        var userId = "test-user";
        var basket = new Basket { UserId = userId };
        var basketDto = new BasketDto { UserId = userId };

        _basketRepository.GetBasketAsync(userId).Returns(basket);
        _mapper.Map<BasketDto>(basket).Returns(basketDto);

        var query = new GetBasketQuery { UserId = userId };

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        await _basketRepository.Received(1).GetBasketAsync(Arg.Is<string>(u => u == userId));
    }
}
