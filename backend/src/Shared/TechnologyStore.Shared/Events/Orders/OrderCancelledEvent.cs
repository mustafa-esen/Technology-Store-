namespace TechnologyStore.Shared.Events.Orders;

public class OrderCancelledEvent
{
    public Guid OrderId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public DateTime CancelledDate { get; set; }
}
