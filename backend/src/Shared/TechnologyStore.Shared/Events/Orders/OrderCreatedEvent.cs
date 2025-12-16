namespace TechnologyStore.Shared.Events.Orders;

/// Sipariş oluşturulduğunda yayınlanan event
/// Publisher: OrderService
/// Consumers: PaymentService, NotificationService, ProductService (Stok)
public interface IOrderCreatedEvent
{
    Guid OrderId { get; set; }
    string UserId { get; set; }
    decimal TotalAmount { get; set; }
    List<OrderItemDto> Items { get; set; }
    ShippingAddressDto ShippingAddress { get; set; }
    DateTime CreatedDate { get; set; }
}

public class OrderItemDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Subtotal { get; set; }
}

public class ShippingAddressDto
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}
