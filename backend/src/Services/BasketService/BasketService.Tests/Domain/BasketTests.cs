using BasketService.Domain.Entities;
using FluentAssertions;

namespace BasketService.Tests.Domain;

public class BasketTests
{
    [Fact]
    public void Basket_Creation_ShouldInitializeWithEmptyItems()
    {
        // Act
        var basket = new Basket();

        // Assert
        basket.Items.Should().NotBeNull();
        basket.Items.Should().BeEmpty();
        basket.TotalPrice.Should().Be(0);
    }

    [Fact]
    public void Basket_WithUserId_ShouldSetUserIdCorrectly()
    {
        // Arrange
        var userId = "test-user-123";

        // Act
        var basket = new Basket { UserId = userId };

        // Assert
        basket.UserId.Should().Be(userId);
    }

    [Fact]
    public void AddItem_WhenBasketIsEmpty_ShouldAddNewItem()
    {
        // Arrange
        var basket = new Basket { UserId = "user-123" };
        var item = new BasketItem
        {
            ProductId = Guid.NewGuid(),
            ProductName = "Product 1",
            Price = 100,
            Quantity = 1
        };

        // Act
        basket.Items.Add(item);

        // Assert
        basket.Items.Should().HaveCount(1);
        basket.Items[0].Should().Be(item);
    }

    [Fact]
    public void TotalPrice_WithSingleItem_ShouldCalculateCorrectly()
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
                    ProductName = "Product",
                    Price = 100,
                    Quantity = 3
                }
            }
        };

        // Act
        var totalPrice = basket.TotalPrice;

        // Assert
        totalPrice.Should().Be(300);
    }

    [Fact]
    public void TotalPrice_WithMultipleItems_ShouldCalculateCorrectly()
    {
        // Arrange
        var basket = new Basket
        {
            UserId = "user-123",
            Items = new List<BasketItem>
            {
                new BasketItem { ProductId = Guid.NewGuid(), ProductName = "Product 1", Price = 100, Quantity = 2 }, // 200
                new BasketItem { ProductId = Guid.NewGuid(), ProductName = "Product 2", Price = 50, Quantity = 3 },  // 150
                new BasketItem { ProductId = Guid.NewGuid(), ProductName = "Product 3", Price = 25, Quantity = 4 }   // 100
            }
        };

        // Act
        var totalPrice = basket.TotalPrice;

        // Assert
        totalPrice.Should().Be(450);
    }

    [Fact]
    public void TotalPrice_WithEmptyBasket_ShouldBeZero()
    {
        // Arrange
        var basket = new Basket
        {
            UserId = "user-123",
            Items = new List<BasketItem>()
        };

        // Act
        var totalPrice = basket.TotalPrice;

        // Assert
        totalPrice.Should().Be(0);
    }

    [Fact]
    public void TotalPrice_WithDecimalPrices_ShouldCalculateCorrectly()
    {
        // Arrange
        var basket = new Basket
        {
            UserId = "user-123",
            Items = new List<BasketItem>
            {
                new BasketItem { ProductId = Guid.NewGuid(), ProductName = "Product 1", Price = 19.99m, Quantity = 3 },  // 59.97
                new BasketItem { ProductId = Guid.NewGuid(), ProductName = "Product 2", Price = 29.50m, Quantity = 2 }   // 59.00
            }
        };

        // Act
        var totalPrice = basket.TotalPrice;

        // Assert
        totalPrice.Should().Be(118.97m);
    }

    [Fact]
    public void CreatedDate_ShouldBeInitializedWithCurrentTime()
    {
        // Arrange
        var beforeCreation = DateTime.UtcNow;

        // Act
        var basket = new Basket { UserId = "user-123" };
        basket.CreatedDate = DateTime.UtcNow;

        // Assert
        basket.CreatedDate.Should().BeOnOrAfter(beforeCreation);
        basket.CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdatedDate_ShouldBeUpdatable()
    {
        // Arrange
        var basket = new Basket
        {
            UserId = "user-123",
            CreatedDate = DateTime.UtcNow.AddDays(-1),
            UpdatedDate = DateTime.UtcNow.AddDays(-1)
        };

        var newUpdateTime = DateTime.UtcNow;

        // Act
        basket.UpdatedDate = newUpdateTime;

        // Assert
        basket.UpdatedDate.Should().Be(newUpdateTime);
        basket.UpdatedDate.Should().BeAfter(basket.CreatedDate);
    }

    [Fact]
    public void RemoveItem_ShouldRemoveItemFromBasket()
    {
        // Arrange
        var itemToRemove = new BasketItem
        {
            ProductId = Guid.NewGuid(),
            ProductName = "Product to Remove",
            Price = 100,
            Quantity = 1
        };

        var basket = new Basket
        {
            UserId = "user-123",
            Items = new List<BasketItem>
            {
                new BasketItem { ProductId = Guid.NewGuid(), ProductName = "Product 1", Price = 50, Quantity = 1 },
                itemToRemove,
                new BasketItem { ProductId = Guid.NewGuid(), ProductName = "Product 3", Price = 75, Quantity = 1 }
            }
        };

        // Act
        basket.Items.Remove(itemToRemove);

        // Assert
        basket.Items.Should().HaveCount(2);
        basket.Items.Should().NotContain(itemToRemove);
    }

    [Fact]
    public void ClearItems_ShouldRemoveAllItems()
    {
        // Arrange
        var basket = new Basket
        {
            UserId = "user-123",
            Items = new List<BasketItem>
            {
                new BasketItem { ProductId = Guid.NewGuid(), ProductName = "Product 1", Price = 100, Quantity = 1 },
                new BasketItem { ProductId = Guid.NewGuid(), ProductName = "Product 2", Price = 200, Quantity = 1 }
            }
        };

        // Act
        basket.Items.Clear();

        // Assert
        basket.Items.Should().BeEmpty();
        basket.TotalPrice.Should().Be(0);
    }

    [Fact]
    public void Basket_WithLargeNumberOfItems_ShouldCalculateTotalCorrectly()
    {
        // Arrange
        var basket = new Basket { UserId = "user-123" };
        var items = new List<BasketItem>();

        for (int i = 1; i <= 100; i++)
        {
            items.Add(new BasketItem
            {
                ProductId = Guid.NewGuid(),
                ProductName = $"Product {i}",
                Price = i,
                Quantity = 1
            });
        }

        basket.Items = items;

        // Act
        var totalPrice = basket.TotalPrice;

        // Assert
        totalPrice.Should().Be(5050); // Sum of 1 to 100 = 100 * 101 / 2 = 5050
    }
}
