using Microsoft.Extensions.Logging;
using PaymentService.Application.Interfaces;

namespace PaymentService.Infrastructure.Services;

/// Fake payment gateway for development/testing purposes.
/// Simulates payment processing without real bank integration.
public class FakePaymentGateway : IPaymentGateway
{
    private readonly ILogger<FakePaymentGateway> _logger;
    private readonly Random _random = new();

    public FakePaymentGateway(ILogger<FakePaymentGateway> logger)
    {
        _logger = logger;
    }

    public async Task<PaymentGatewayResult> ProcessPaymentAsync(string orderId, decimal amount, string userId)
    {
        _logger.LogInformation("🏦 FakePaymentGateway: Processing payment for OrderId: {OrderId}, Amount: {Amount}, UserId: {UserId}",
            orderId, amount, userId);

        // Simulate network delay
        await Task.Delay(1000);

        // Simulate 90% success rate
        var isSuccess = _random.Next(1, 11) <= 9;

        if (isSuccess)
        {
            var transactionId = $"TXN-{Guid.NewGuid().ToString()[..8].ToUpper()}";
            _logger.LogInformation("✅ FakePaymentGateway: Payment successful. TransactionId: {TransactionId}", transactionId);

            return new PaymentGatewayResult
            {
                IsSuccess = true,
                TransactionId = transactionId
            };
        }
        else
        {
            var failureReasons = new[]
            {
                "Insufficient funds",
                "Card declined",
                "Invalid card details",
                "Bank timeout",
                "Daily limit exceeded"
            };

            var reason = failureReasons[_random.Next(failureReasons.Length)];
            _logger.LogWarning("❌ FakePaymentGateway: Payment failed. Reason: {Reason}", reason);

            return new PaymentGatewayResult
            {
                IsSuccess = false,
                FailureReason = reason
            };
        }
    }
}
