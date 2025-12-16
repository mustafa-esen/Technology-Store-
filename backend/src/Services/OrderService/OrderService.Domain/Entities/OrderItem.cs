namespace OrderService.Domain.Entities;

public class OrderItem
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }

    private decimal _subtotal;
    public decimal Subtotal
    {
        get => Price * Quantity;
        set => _subtotal = value;
    }

    // Navigation property
    public Order Order { get; set; } = null!;

    public OrderItem()
    {
        Id = Guid.NewGuid();
    }

    public OrderItem(Guid productId, string productName, decimal price, int quantity)
    {
        Id = Guid.NewGuid();
        ProductId = productId;
        ProductName = productName;
        Price = price;
        Quantity = quantity;
    }
}
