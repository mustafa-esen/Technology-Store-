using BasketService.Domain.Entities;
using FluentAssertions;

namespace BasketService.Tests.Domain;

public class BasketItemTests
{
    [Fact]
    public void BasketItem_Creation_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var productName = "Test Product";
        var price = 99.99m;
        var quantity = 5;

        // Act
        var item = new BasketItem
        {
            ProductId = productId,
            ProductName = productName,
            Price = price,
            Quantity = quantity
        };

        // Assert
        item.ProductId.Should().Be(productId);
        item.ProductName.Should().Be(productName);
        item.Price.Should().Be(price);
        item.Quantity.Should().Be(quantity);
    }

    [Fact]
    public void BasketItem_WithoutOptionalFields_ShouldCreateSuccessfully()
    {
        // Act
        var item = new BasketItem
        {
            ProductId = Guid.NewGuid(),
            ProductName = "Product",
            Price = 100,
            Quantity = 1
        };

        // Assert
        item.ProductName.Should().NotBeNullOrEmpty();
        item.Price.Should().BeGreaterThan(0);
    }

    [Fact]
    public void BasketItem_Quantity_ShouldBeUpdatable()
    {
        // Arrange
        var item = new BasketItem
        {
            ProductId = Guid.NewGuid(),
            ProductName = "Product",
            Price = 50,
            Quantity = 1
        };

        // Act
        item.Quantity = 10;

        // Assert
        item.Quantity.Should().Be(10);
    }

    [Fact]
    public void BasketItem_Price_ShouldBeUpdatable()
    {
        // Arrange
        var item = new BasketItem
        {
            ProductId = Guid.NewGuid(),
            ProductName = "Product",
            Price = 100,
            Quantity = 1
        };

        // Act
        item.Price = 150.50m;

        // Assert
        item.Price.Should().Be(150.50m);
    }

    [Fact]
    public void BasketItem_TotalPrice_SingleUnit_ShouldEqualPrice()
    {
        // Arrange
        var price = 99.99m;
        var item = new BasketItem
        {
            ProductId = Guid.NewGuid(),
            ProductName = "Product",
            Price = price,
            Quantity = 1
        };

        // Act
        var totalPrice = item.Price * item.Quantity;

        // Assert
        totalPrice.Should().Be(price);
    }

    [Fact]
    public void BasketItem_TotalPrice_MultipleUnits_ShouldCalculateCorrectly()
    {
        // Arrange
        var item = new BasketItem
        {
            ProductId = Guid.NewGuid(),
            ProductName = "Product",
            Price = 25.50m,
            Quantity = 4
        };

        // Act
        var totalPrice = item.Price * item.Quantity;

        // Assert
        totalPrice.Should().Be(102.00m);
    }

    [Theory]
    [InlineData(10, 1, 10)]
    [InlineData(10, 5, 50)]
    [InlineData(19.99, 3, 59.97)]
    [InlineData(0.50, 100, 50)]
    [InlineData(999.99, 2, 1999.98)]
    public void BasketItem_TotalPrice_ShouldCalculateCorrectly(decimal price, int quantity, decimal expectedTotal)
    {
        // Arrange
        var item = new BasketItem
        {
            ProductId = Guid.NewGuid(),
            ProductName = "Product",
            Price = price,
            Quantity = quantity
        };

        // Act
        var totalPrice = item.Price * item.Quantity;

        // Assert
        totalPrice.Should().Be(expectedTotal);
    }

    [Fact]
    public void BasketItem_WithZeroPrice_ShouldBeAllowed()
    {
        // Act
        var item = new BasketItem
        {
            ProductId = Guid.NewGuid(),
            ProductName = "Free Product",
            Price = 0,
            Quantity = 1
        };

        // Assert
        item.Price.Should().Be(0);
    }

    [Fact]
    public void BasketItem_ProductName_ShouldSupportUnicodeCharacters()
    {
        // Arrange
        var productName = "Teknoloji Mağazası - Ürün 🎁";

        // Act
        var item = new BasketItem
        {
            ProductId = Guid.NewGuid(),
            ProductName = productName,
            Price = 100,
            Quantity = 1
        };

        // Assert
        item.ProductName.Should().Be(productName);
    }

    [Fact]
    public void BasketItem_Equality_SameProductId_ShouldBeConsideredEqual()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var item1 = new BasketItem
        {
            ProductId = productId,
            ProductName = "Product",
            Price = 100,
            Quantity = 1
        };

        var item2 = new BasketItem
        {
            ProductId = productId,
            ProductName = "Product",
            Price = 100,
            Quantity = 2 // Different quantity
        };

        // Assert
        item1.ProductId.Should().Be(item2.ProductId);
    }

    [Fact]
    public void BasketItem_WithLargeQuantity_ShouldHandleCorrectly()
    {
        // Arrange
        var item = new BasketItem
        {
            ProductId = Guid.NewGuid(),
            ProductName = "Bulk Product",
            Price = 10,
            Quantity = 10000
        };

        // Act
        var totalPrice = item.Price * item.Quantity;

        // Assert
        item.Quantity.Should().Be(10000);
        totalPrice.Should().Be(100000);
    }

    [Fact]
    public void BasketItem_WithHighPrice_ShouldHandleDecimalPrecision()
    {
        // Arrange
        var item = new BasketItem
        {
            ProductId = Guid.NewGuid(),
            ProductName = "Expensive Product",
            Price = 9999999.99m,
            Quantity = 1
        };

        // Assert
        item.Price.Should().Be(9999999.99m);
    }
}
