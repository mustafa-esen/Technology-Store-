namespace PaymentService.Application.Interfaces;

public interface IPaymentGateway
{
    Task<PaymentGatewayResult> ProcessPaymentAsync(string orderId, decimal amount, string userId);
}

public class PaymentGatewayResult
{
    public bool IsSuccess { get; set; }
    public string? TransactionId { get; set; }
    public string? FailureReason { get; set; }
}
