using FluentAssertions;
using FluentValidation.TestHelper;
using OrderService.Application.Features.Orders.Commands.UpdateOrderStatus;

namespace OrderService.Tests.Validators;

public class UpdateOrderStatusCommandValidatorTests
{
    private readonly UpdateOrderStatusCommandValidator _validator;

    public UpdateOrderStatusCommandValidatorTests()
    {
        _validator = new UpdateOrderStatusCommandValidator();
    }

    [Fact]
    public void Validate_ShouldPass_WithValidCommand()
    {
        // Arrange
        var command = new UpdateOrderStatusCommand
        {
            OrderId = Guid.NewGuid(),
            Status = "Processing"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldFail_WhenOrderIdIsEmpty()
    {
        // Arrange
        var command = new UpdateOrderStatusCommand
        {
            OrderId = Guid.Empty,
            Status = "Processing"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.OrderId)
            .WithErrorMessage("OrderId is required.");
    }

    [Fact]
    public void Validate_ShouldFail_WhenStatusIsEmpty()
    {
        // Arrange
        var command = new UpdateOrderStatusCommand
        {
            OrderId = Guid.NewGuid(),
            Status = string.Empty
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Status)
            .WithErrorMessage("Status is required.");
    }

    [Theory]
    [InlineData("Pending")]
    [InlineData("PaymentReceived")]
    [InlineData("Processing")]
    [InlineData("Shipped")]
    [InlineData("Delivered")]
    [InlineData("Cancelled")]
    public void Validate_ShouldPass_WithValidStatusValues(string status)
    {
        // Arrange
        var command = new UpdateOrderStatusCommand
        {
            OrderId = Guid.NewGuid(),
            Status = status
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Status);
    }

    [Theory]
    [InlineData("pending")]
    [InlineData("PROCESSING")]
    [InlineData("sHiPpEd")]
    public void Validate_ShouldPass_WithCaseInsensitiveStatusValues(string status)
    {
        // Arrange
        var command = new UpdateOrderStatusCommand
        {
            OrderId = Guid.NewGuid(),
            Status = status
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Status);
    }

    [Theory]
    [InlineData("InvalidStatus")]
    [InlineData("Unknown")]
    [InlineData("Completed")]
    public void Validate_ShouldFail_WithInvalidStatusValues(string status)
    {
        // Arrange
        var command = new UpdateOrderStatusCommand
        {
            OrderId = Guid.NewGuid(),
            Status = status
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Status);
    }
}
