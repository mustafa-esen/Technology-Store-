using FluentAssertions;
using OrderService.Domain.Entities;

namespace OrderService.Tests.Domain;

public class OrderItemTests
{
    [Fact]
    public void Constructor_ShouldCreateOrderItem_WithValidData()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var productName = "Test Product";
        var price = 99.99m;
        var quantity = 3;

        // Act
        var orderItem = new OrderItem(productId, productName, price, quantity);

        // Assert
        orderItem.Id.Should().NotBeEmpty();
        orderItem.ProductId.Should().Be(productId);
        orderItem.ProductName.Should().Be(productName);
        orderItem.Price.Should().Be(price);
        orderItem.Quantity.Should().Be(quantity);
    }

    [Theory]
    [InlineData(100, 1, 100)]
    [InlineData(50.5, 2, 101)]
    [InlineData(25.25, 4, 101)]
    [InlineData(10, 10, 100)]
    public void Subtotal_ShouldCalculateCorrectly(decimal price, int quantity, decimal expectedSubtotal)
    {
        // Arrange & Act
        var orderItem = new OrderItem(Guid.NewGuid(), "Product", price, quantity);

        // Assert
        orderItem.Subtotal.Should().Be(expectedSubtotal);
    }

    [Fact]
    public void Subtotal_ShouldUpdateDynamically_WhenPriceChanges()
    {
        // Arrange
        var orderItem = new OrderItem(Guid.NewGuid(), "Product", 100m, 2);

        // Act
        orderItem.Price = 150m;

        // Assert
        orderItem.Subtotal.Should().Be(300m);
    }

    [Fact]
    public void Subtotal_ShouldUpdateDynamically_WhenQuantityChanges()
    {
        // Arrange
        var orderItem = new OrderItem(Guid.NewGuid(), "Product", 50m, 2);

        // Act
        orderItem.Quantity = 5;

        // Assert
        orderItem.Subtotal.Should().Be(250m);
    }
}
