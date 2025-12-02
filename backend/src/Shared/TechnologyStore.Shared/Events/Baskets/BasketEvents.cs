namespace TechnologyStore.Shared.Events.Baskets;

/// Sepet onaylandığında (Checkout) yayınlanan event
/// Publisher: BasketService
/// Consumer: OrderService
public interface IBasketCheckoutEvent
{
    string UserId { get; set; }
    string UserName { get; set; }
    decimal TotalPrice { get; set; }
    BasketCheckoutAddressDto ShippingAddress { get; set; }
    List<BasketItemDto> Items { get; set; }
    DateTime CheckedOutDate { get; set; }
}

public class BasketItemDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

public class BasketCheckoutAddressDto
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}
