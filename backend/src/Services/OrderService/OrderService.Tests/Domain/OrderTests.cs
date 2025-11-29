using FluentAssertions;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;
using OrderService.Domain.ValueObjects;

namespace OrderService.Tests.Domain;

public class OrderTests
{
    [Fact]
    public void Constructor_ShouldCreateOrder_WithValidData()
    {
        // Arrange
        var userId = "user-123";
        var shippingAddress = new Address("123 Street", "City", "State", "12345", "Country");

        // Act
        var order = new Order(userId, shippingAddress);

        // Assert
        order.UserId.Should().Be(userId);
        order.ShippingAddress.Should().Be(shippingAddress);
        order.Status.Should().Be(OrderStatus.Pending);
        order.Items.Should().BeEmpty();
        order.TotalAmount.Should().Be(0);
        order.CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        order.UpdatedDate.Should().NotBeNull();
    }

    [Fact]
    public void AddItem_ShouldAddNewItem_ToOrder()
    {
        // Arrange
        var order = CreateTestOrder();
        var item = new OrderItem(Guid.NewGuid(), "Test Product", 100m, 2);

        // Act
        order.AddItem(item);

        // Assert
        order.Items.Should().HaveCount(1);
        order.Items.First().ProductName.Should().Be("Test Product");
        order.Items.First().Price.Should().Be(100m);
        order.Items.First().Quantity.Should().Be(2);
        order.Items.First().Subtotal.Should().Be(200m);
    }

    [Fact]
    public void TotalAmount_ShouldCalculateCorrectly_WithMultipleItems()
    {
        // Arrange
        var order = CreateTestOrder();
        order.AddItem(new OrderItem(Guid.NewGuid(), "Product 1", 100m, 2)); // 200
        order.AddItem(new OrderItem(Guid.NewGuid(), "Product 2", 50m, 3));  // 150

        // Act
        var total = order.TotalAmount;

        // Assert
        total.Should().Be(350m);
    }

    [Fact]
    public void UpdateStatus_ShouldChangeStatus()
    {
        // Arrange
        var order = CreateTestOrder();

        // Act
        order.UpdateStatus(OrderStatus.Processing);

        // Assert
        order.Status.Should().Be(OrderStatus.Processing);
        order.UpdatedDate.Should().NotBeNull();
    }

    [Fact]
    public void UpdateStatus_ToDelivered_ShouldSetCompletedDate()
    {
        // Arrange
        var order = CreateTestOrder();

        // Act
        order.UpdateStatus(OrderStatus.Delivered);

        // Assert
        order.Status.Should().Be(OrderStatus.Delivered);
        order.CompletedDate.Should().NotBeNull();
        order.CompletedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void UpdateStatus_ToCancelled_ShouldSetCancelledDate()
    {
        // Arrange
        var order = CreateTestOrder();

        // Act
        order.UpdateStatus(OrderStatus.Cancelled);

        // Assert
        order.Status.Should().Be(OrderStatus.Cancelled);
        order.CancelledDate.Should().NotBeNull();
        order.CancelledDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Cancel_ShouldSetStatusToCancelled_AndSetCancelledDate()
    {
        // Arrange
        var order = CreateTestOrder();
        var reason = "Customer request";

        // Act
        order.Cancel(reason);

        // Assert
        order.Status.Should().Be(OrderStatus.Cancelled);
        order.CancellationReason.Should().Be(reason);
        order.CancelledDate.Should().NotBeNull();
        order.CancelledDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void SetPaymentInfo_ShouldSetPaymentDetails_AndUpdateStatus()
    {
        // Arrange
        var order = CreateTestOrder();
        var paymentIntentId = "pi_123456";
        var paymentMethod = "Credit Card";

        // Act
        order.SetPaymentInfo(paymentIntentId, paymentMethod);

        // Assert
        order.PaymentIntentId.Should().Be(paymentIntentId);
        order.PaymentMethod.Should().Be(paymentMethod);
        order.Status.Should().Be(OrderStatus.PaymentReceived);
    }

    [Theory]
    [InlineData(OrderStatus.Pending, true)]
    [InlineData(OrderStatus.PaymentReceived, true)]
    [InlineData(OrderStatus.Processing, true)]
    [InlineData(OrderStatus.Shipped, false)]
    [InlineData(OrderStatus.Delivered, false)]
    [InlineData(OrderStatus.Cancelled, false)]
    public void CanBeCancelled_ShouldReturnExpectedResult(OrderStatus status, bool expectedResult)
    {
        // Arrange
        var order = CreateTestOrder();
        order.UpdateStatus(status);

        // Act
        var result = order.CanBeCancelled();

        // Assert
        result.Should().Be(expectedResult);
    }

    private static Order CreateTestOrder()
    {
        var address = new Address("Test Street", "Test City", "Test State", "12345", "Test Country");
        return new Order("test-user", address);
    }
}
