namespace TechnologyStore.Shared.Events.Orders;

public class OrderCompletedEvent
{
    public Guid OrderId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime CompletedDate { get; set; }
}
