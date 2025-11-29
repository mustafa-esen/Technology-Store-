namespace TechnologyStore.Shared.Events.Baskets;

public class BasketCheckedOutEvent
{
    public string UserId { get; set; } = string.Empty;
    public List<BasketItemDto> Items { get; set; } = new();
    public decimal TotalPrice { get; set; }
    public DateTime CheckedOutDate { get; set; }
}

public class BasketItemDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}
