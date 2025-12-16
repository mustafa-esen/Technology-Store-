using FluentAssertions;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Enums;
using PaymentService.Domain.ValueObjects;

namespace PaymentService.Tests.Domain.Entities;

public class PaymentTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreatePayment()
    {
        // Arrange
        var orderId = "order-123";
        var userId = "user-456";
        var amount = new Money(100m, "TRY");

        // Act
        var payment = new Payment(orderId, userId, amount);

        // Assert
        payment.Id.Should().NotBeEmpty();
        payment.OrderId.Should().Be(orderId);
        payment.UserId.Should().Be(userId);
        payment.Amount.Should().Be(amount);
        payment.Status.Should().Be(PaymentStatus.Pending);
        payment.CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        payment.TransactionId.Should().BeNull();
        payment.FailureReason.Should().BeNull();
        payment.ProcessedDate.Should().BeNull();
    }

    [Fact]
    public void Constructor_WithEmptyOrderId_ShouldThrowArgumentException()
    {
        // Arrange
        var emptyOrderId = string.Empty;
        var userId = "user-456";
        var amount = new Money(100m, "TRY");

        // Act
        Action act = () => new Payment(emptyOrderId, userId, amount);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*OrderId cannot be empty*");
    }

    [Fact]
    public void Constructor_WithNullOrderId_ShouldThrowArgumentException()
    {
        // Arrange
        string? nullOrderId = null;
        var userId = "user-456";
        var amount = new Money(100m, "TRY");

        // Act
        Action act = () => new Payment(nullOrderId!, userId, amount);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*OrderId cannot be empty*");
    }

    [Fact]
    public void Constructor_WithEmptyUserId_ShouldThrowArgumentException()
    {
        // Arrange
        var orderId = "order-123";
        var emptyUserId = string.Empty;
        var amount = new Money(100m, "TRY");

        // Act
        Action act = () => new Payment(orderId, emptyUserId, amount);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*UserId cannot be empty*");
    }

    [Fact]
    public void Constructor_WithNullUserId_ShouldThrowArgumentException()
    {
        // Arrange
        var orderId = "order-123";
        string? nullUserId = null;
        var amount = new Money(100m, "TRY");

        // Act
        Action act = () => new Payment(orderId, nullUserId!, amount);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*UserId cannot be empty*");
    }

    [Fact]
    public void Constructor_WithNullAmount_ShouldThrowArgumentNullException()
    {
        // Arrange
        var orderId = "order-123";
        var userId = "user-456";
        Money? nullAmount = null;

        // Act
        Action act = () => new Payment(orderId, userId, nullAmount!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void MarkAsProcessing_WhenPending_ShouldChangeStatusToProcessing()
    {
        // Arrange
        var payment = new Payment("order-123", "user-456", new Money(100m, "TRY"));

        // Act
        payment.MarkAsProcessing();

        // Assert
        payment.Status.Should().Be(PaymentStatus.Processing);
    }

    [Fact]
    public void MarkAsProcessing_WhenNotPending_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var payment = new Payment("order-123", "user-456", new Money(100m, "TRY"));
        payment.MarkAsProcessing(); // Already processing

        // Act
        Action act = () => payment.MarkAsProcessing();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot mark payment as processing when status is*");
    }

    [Fact]
    public void MarkAsSuccess_WhenProcessing_ShouldChangeStatusToSuccessAndSetTransactionId()
    {
        // Arrange
        var payment = new Payment("order-123", "user-456", new Money(100m, "TRY"));
        payment.MarkAsProcessing();
        var transactionId = "txn-789";

        // Act
        payment.MarkAsSuccess(transactionId);

        // Assert
        payment.Status.Should().Be(PaymentStatus.Success);
        payment.TransactionId.Should().Be(transactionId);
        payment.ProcessedDate.Should().NotBeNull();
        payment.ProcessedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void MarkAsSuccess_WhenNotProcessing_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var payment = new Payment("order-123", "user-456", new Money(100m, "TRY"));
        var transactionId = "txn-789";

        // Act
        Action act = () => payment.MarkAsSuccess(transactionId);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot mark payment as success when status is*");
    }

    [Fact]
    public void MarkAsSuccess_WithEmptyTransactionId_ShouldThrowArgumentException()
    {
        // Arrange
        var payment = new Payment("order-123", "user-456", new Money(100m, "TRY"));
        payment.MarkAsProcessing();
        var emptyTransactionId = string.Empty;

        // Act
        Action act = () => payment.MarkAsSuccess(emptyTransactionId);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*TransactionId cannot be empty*");
    }

    [Fact]
    public void MarkAsSuccess_WithNullTransactionId_ShouldThrowArgumentException()
    {
        // Arrange
        var payment = new Payment("order-123", "user-456", new Money(100m, "TRY"));
        payment.MarkAsProcessing();
        string? nullTransactionId = null;

        // Act
        Action act = () => payment.MarkAsSuccess(nullTransactionId!);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*TransactionId cannot be empty*");
    }

    [Fact]
    public void MarkAsFailed_WhenProcessing_ShouldChangeStatusToFailedAndSetFailureReason()
    {
        // Arrange
        var payment = new Payment("order-123", "user-456", new Money(100m, "TRY"));
        payment.MarkAsProcessing();
        var failureReason = "Insufficient funds";

        // Act
        payment.MarkAsFailed(failureReason);

        // Assert
        payment.Status.Should().Be(PaymentStatus.Failed);
        payment.FailureReason.Should().Be(failureReason);
        payment.ProcessedDate.Should().NotBeNull();
        payment.ProcessedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void MarkAsFailed_WhenNotProcessing_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var payment = new Payment("order-123", "user-456", new Money(100m, "TRY"));
        var failureReason = "Insufficient funds";

        // Act
        Action act = () => payment.MarkAsFailed(failureReason);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot mark payment as failed when status is*");
    }

    [Fact]
    public void MarkAsFailed_WithEmptyFailureReason_ShouldThrowArgumentException()
    {
        // Arrange
        var payment = new Payment("order-123", "user-456", new Money(100m, "TRY"));
        payment.MarkAsProcessing();
        var emptyReason = string.Empty;

        // Act
        Action act = () => payment.MarkAsFailed(emptyReason);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*FailureReason cannot be empty*");
    }

    [Fact]
    public void MarkAsFailed_WithNullFailureReason_ShouldThrowArgumentException()
    {
        // Arrange
        var payment = new Payment("order-123", "user-456", new Money(100m, "TRY"));
        payment.MarkAsProcessing();
        string? nullReason = null;

        // Act
        Action act = () => payment.MarkAsFailed(nullReason!);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*FailureReason cannot be empty*");
    }

    [Fact]
    public void MarkAsRefunded_WhenSuccess_ShouldChangeStatusToRefunded()
    {
        // Arrange
        var payment = new Payment("order-123", "user-456", new Money(100m, "TRY"));
        payment.MarkAsProcessing();
        payment.MarkAsSuccess("txn-789");
        var refundReason = "Customer request";

        // Act
        payment.MarkAsRefunded(refundReason);

        // Assert
        payment.Status.Should().Be(PaymentStatus.Refunded);
        payment.FailureReason.Should().Be(refundReason);
    }

    [Fact]
    public void MarkAsRefunded_WhenNotSuccess_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var payment = new Payment("order-123", "user-456", new Money(100m, "TRY"));
        var refundReason = "Customer request";

        // Act
        Action act = () => payment.MarkAsRefunded(refundReason);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Can only refund successful payments*");
    }

    [Fact]
    public void IsOrderAlreadyProcessed_WhenOrderNotProcessed_ShouldReturnFalse()
    {
        // Arrange
        var orderId = "order-new-999";

        // Act
        var result = Payment.IsOrderAlreadyProcessed(orderId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsOrderAlreadyProcessed_AfterSuccessfulPayment_ShouldReturnTrue()
    {
        // Arrange
        var orderId = "order-idempotency-test";
        var payment = new Payment(orderId, "user-456", new Money(100m, "TRY"));
        payment.MarkAsProcessing();
        payment.MarkAsSuccess("txn-789");

        // Act
        var result = Payment.IsOrderAlreadyProcessed(orderId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void CompletePaymentFlow_Success_ShouldTransitionThroughAllStates()
    {
        // Arrange
        var orderId = "order-flow-test";
        var userId = "user-flow";
        var amount = new Money(250m, "TRY");
        var transactionId = "txn-success-123";

        // Act & Assert - Step 1: Create
        var payment = new Payment(orderId, userId, amount);
        payment.Status.Should().Be(PaymentStatus.Pending);
        payment.TransactionId.Should().BeNull();
        payment.ProcessedDate.Should().BeNull();

        // Act & Assert - Step 2: Start Processing
        payment.MarkAsProcessing();
        payment.Status.Should().Be(PaymentStatus.Processing);

        // Act & Assert - Step 3: Mark Success
        payment.MarkAsSuccess(transactionId);
        payment.Status.Should().Be(PaymentStatus.Success);
        payment.TransactionId.Should().Be(transactionId);
        payment.ProcessedDate.Should().NotBeNull();

        // Act & Assert - Step 4: Verify Idempotency
        var isProcessed = Payment.IsOrderAlreadyProcessed(orderId);
        isProcessed.Should().BeTrue();
    }

    [Fact]
    public void CompletePaymentFlow_Failure_ShouldTransitionToFailedState()
    {
        // Arrange
        var orderId = "order-fail-test";
        var userId = "user-fail";
        var amount = new Money(150m, "TRY");
        var failureReason = "Card declined";

        // Act & Assert - Step 1: Create
        var payment = new Payment(orderId, userId, amount);
        payment.Status.Should().Be(PaymentStatus.Pending);

        // Act & Assert - Step 2: Start Processing
        payment.MarkAsProcessing();
        payment.Status.Should().Be(PaymentStatus.Processing);

        // Act & Assert - Step 3: Mark Failed
        payment.MarkAsFailed(failureReason);
        payment.Status.Should().Be(PaymentStatus.Failed);
        payment.FailureReason.Should().Be(failureReason);
        payment.ProcessedDate.Should().NotBeNull();

        // Act & Assert - Step 4: Verify NOT in Idempotency list (only successful payments)
        var isProcessed = Payment.IsOrderAlreadyProcessed(orderId);
        isProcessed.Should().BeFalse();
    }
}
