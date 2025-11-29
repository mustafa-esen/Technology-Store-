using OrderService.Domain.Enums;
using OrderService.Domain.ValueObjects;

namespace OrderService.Domain.Entities;

public class Order
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public OrderStatus Status { get; set; }
    public List<OrderItem> Items { get; set; } = new();

    private decimal _totalAmount;
    public decimal TotalAmount
    {
        get => Items.Any() ? Items.Sum(item => item.Subtotal) : _totalAmount;
        set => _totalAmount = value;
    }

    // Shipping Address
    public Address ShippingAddress { get; set; } = null!;

    // Timestamps
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public DateTime? CancelledDate { get; set; }

    // Payment info
    public string? PaymentIntentId { get; set; }
    public string? PaymentMethod { get; set; }

    // Additional info
    public string? Notes { get; set; }
    public string? CancellationReason { get; set; }

    public Order()
    {
        Id = Guid.NewGuid();
        Status = OrderStatus.Pending;
        CreatedDate = DateTime.UtcNow;
        UpdatedDate = DateTime.UtcNow;
    }

    public Order(string userId, Address shippingAddress)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        ShippingAddress = shippingAddress;
        Status = OrderStatus.Pending;
        Items = new List<OrderItem>();
        CreatedDate = DateTime.UtcNow;
        UpdatedDate = DateTime.UtcNow;
    }

    public void AddItem(OrderItem item)
    {
        Items.Add(item);
        UpdatedDate = DateTime.UtcNow;
    }

    public void UpdateStatus(OrderStatus newStatus)
    {
        Status = newStatus;
        UpdatedDate = DateTime.UtcNow;

        if (newStatus == OrderStatus.Delivered)
        {
            CompletedDate = DateTime.UtcNow;
        }
        else if (newStatus == OrderStatus.Cancelled)
        {
            CancelledDate = DateTime.UtcNow;
        }
    }

    public void Cancel(string reason)
    {
        Status = OrderStatus.Cancelled;
        CancellationReason = reason;
        CancelledDate = DateTime.UtcNow;
        UpdatedDate = DateTime.UtcNow;
    }

    public void SetPaymentInfo(string paymentIntentId, string paymentMethod)
    {
        PaymentIntentId = paymentIntentId;
        PaymentMethod = paymentMethod;
        Status = OrderStatus.PaymentReceived;
        UpdatedDate = DateTime.UtcNow;
    }

    public bool CanBeCancelled()
    {
        return Status == OrderStatus.Pending ||
               Status == OrderStatus.PaymentReceived ||
               Status == OrderStatus.Processing;
    }
}
