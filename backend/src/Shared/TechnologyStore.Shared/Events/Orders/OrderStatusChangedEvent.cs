namespace TechnologyStore.Shared.Events.Orders;

public enum OrderStatus
{
    Pending = 0,
    PaymentReceived = 1,
    Processing = 2,
    Shipped = 3,
    Delivered = 4,
    Cancelled = 5,
    Failed = 6
}

public class OrderStatusChangedEvent
{
    public Guid OrderId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public OrderStatus OldStatus { get; set; }
    public OrderStatus NewStatus { get; set; }
    public DateTime ChangedDate { get; set; }
}
