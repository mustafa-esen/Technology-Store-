namespace TechnologyStore.Shared.Events.Orders;

/// Sipariş iptal edildiğinde yayınlanan event
/// Publisher: OrderService
/// Consumers: NotificationService, ProductService (Stok iadesi)
public interface IOrderCancelledEvent
{
    Guid OrderId { get; set; }
    string UserId { get; set; }
    string Reason { get; set; }
    DateTime CancelledDate { get; set; }
}
