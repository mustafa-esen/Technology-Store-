using PaymentService.Domain.Enums;

namespace PaymentService.Application.DTOs;

public class PaymentDto
{
    public Guid Id { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public PaymentStatus Status { get; set; }
    public string? TransactionId { get; set; }
    public string? FailureReason { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ProcessedDate { get; set; }
}
