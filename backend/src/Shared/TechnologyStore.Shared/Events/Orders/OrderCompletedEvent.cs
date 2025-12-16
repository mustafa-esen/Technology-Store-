namespace TechnologyStore.Shared.Events.Orders;

/// Sipariş tamamlandığında yayınlanan event
/// Publisher: OrderService
/// Consumer: NotificationService
public interface IOrderCompletedEvent
{
    Guid OrderId { get; set; }
    string UserId { get; set; }
    decimal TotalAmount { get; set; }
    DateTime CompletedDate { get; set; }
}
