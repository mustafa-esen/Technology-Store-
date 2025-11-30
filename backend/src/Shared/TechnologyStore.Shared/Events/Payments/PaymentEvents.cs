namespace TechnologyStore.Shared.Events.Payments;

/// Ödeme talebi oluşturulduğunda yayınlanan event
/// Publisher: OrderService
/// Consumer: PaymentService
public interface IPaymentRequestedEvent
{
    Guid OrderId { get; set; }
    string UserId { get; set; }
    decimal Amount { get; set; }
    string PaymentMethod { get; set; }
    DateTime RequestedDate { get; set; }
}

/// Ödeme başarılı olduğunda yayınlanan event
/// Publisher: PaymentService
/// Consumer: OrderService
public interface IPaymentSuccessEvent
{
    Guid OrderId { get; set; }
    string UserId { get; set; }
    decimal Amount { get; set; }
    string PaymentIntentId { get; set; }
    string PaymentMethod { get; set; }
    DateTime CompletedDate { get; set; }
}

/// Ödeme başarısız olduğunda yayınlanan event
/// Publisher: PaymentService
/// Consumer: OrderService
public interface IPaymentFailedEvent
{
    Guid OrderId { get; set; }
    string UserId { get; set; }
    decimal Amount { get; set; }
    string Reason { get; set; }
    DateTime FailedDate { get; set; }
}