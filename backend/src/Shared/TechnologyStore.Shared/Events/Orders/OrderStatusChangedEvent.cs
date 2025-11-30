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

/// Sipariş durumu değiştiğinde yayınlanan event
/// Publisher: OrderService
/// Consumer: NotificationService
public interface IOrderStatusChangedEvent
{
    Guid OrderId { get; set; }
    string UserId { get; set; }
    OrderStatus OldStatus { get; set; }
    OrderStatus NewStatus { get; set; }
    DateTime ChangedDate { get; set; }
}
