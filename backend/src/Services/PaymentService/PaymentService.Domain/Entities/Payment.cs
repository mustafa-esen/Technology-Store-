using PaymentService.Domain.Enums;
using PaymentService.Domain.ValueObjects;

namespace PaymentService.Domain.Entities;

public class Payment
{
    public Guid Id { get; private set; }
    public string OrderId { get; private set; }
    public string UserId { get; private set; }
    public Money Amount { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string? TransactionId { get; private set; }
    public string? FailureReason { get; private set; }
    public DateTime CreatedDate { get; private set; }
    public DateTime? ProcessedDate { get; private set; }

    private static readonly HashSet<string> ProcessedOrderIds = new();

    private Payment() { } 

    public Payment(string orderId, string userId, Money amount)
    {
        if (string.IsNullOrWhiteSpace(orderId))
            throw new ArgumentException("OrderId cannot be empty", nameof(orderId));

        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("UserId cannot be empty", nameof(userId));

        if (amount == null)
            throw new ArgumentNullException(nameof(amount));

        Id = Guid.NewGuid();
        OrderId = orderId;
        UserId = userId;
        Amount = amount;
        Status = PaymentStatus.Pending;
        CreatedDate = DateTime.UtcNow;
    }

    public static bool IsOrderAlreadyProcessed(string orderId)
    {
        return ProcessedOrderIds.Contains(orderId);
    }

    public void MarkAsProcessing()
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException($"Cannot mark payment as processing when status is {Status}");

        Status = PaymentStatus.Processing;
    }

    public void MarkAsSuccess(string transactionId)
    {
        if (Status != PaymentStatus.Processing)
            throw new InvalidOperationException($"Cannot mark payment as success when status is {Status}");

        if (string.IsNullOrWhiteSpace(transactionId))
            throw new ArgumentException("TransactionId cannot be empty", nameof(transactionId));

        Status = PaymentStatus.Success;
        TransactionId = transactionId;
        ProcessedDate = DateTime.UtcNow;
        ProcessedOrderIds.Add(OrderId);
    }

    public void MarkAsFailed(string failureReason)
    {
        if (Status != PaymentStatus.Processing)
            throw new InvalidOperationException($"Cannot mark payment as failed when status is {Status}");

        if (string.IsNullOrWhiteSpace(failureReason))
            throw new ArgumentException("FailureReason cannot be empty", nameof(failureReason));

        Status = PaymentStatus.Failed;
        FailureReason = failureReason;
        ProcessedDate = DateTime.UtcNow;
    }

    public void MarkAsRefunded(string reason)
    {
        if (Status != PaymentStatus.Success)
            throw new InvalidOperationException("Can only refund successful payments");

        Status = PaymentStatus.Refunded;
        FailureReason = reason;
    }
}
