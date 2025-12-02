using AutoMapper;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;
using NSubstitute;
using PaymentService.Application.DTOs;
using PaymentService.Application.Features.Payments.Commands.ProcessPayment;
using PaymentService.Application.Interfaces;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Enums;
using PaymentService.Domain.ValueObjects;
using TechnologyStore.Shared.Events.Payments;

namespace PaymentService.Tests.Application.Commands;

public class ProcessPaymentCommandHandlerTests
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentGateway _paymentGateway;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IMapper _mapper;
    private readonly ILogger<ProcessPaymentCommandHandler> _logger;
    private readonly ProcessPaymentCommandHandler _handler;

    public ProcessPaymentCommandHandlerTests()
    {
        _paymentRepository = Substitute.For<IPaymentRepository>();
        _paymentGateway = Substitute.For<IPaymentGateway>();
        _publishEndpoint = Substitute.For<IPublishEndpoint>();
        _mapper = Substitute.For<IMapper>();
        _logger = Substitute.For<ILogger<ProcessPaymentCommandHandler>>();

        _handler = new ProcessPaymentCommandHandler(
            _paymentRepository,
            _paymentGateway,
            _publishEndpoint,
            _mapper,
            _logger);
    }

    [Fact]
    public async Task Handle_WithNewOrder_ShouldProcessPaymentSuccessfully()
    {
        // Arrange
        var command = new ProcessPaymentCommand
        {
            OrderId = "order-123",
            UserId = "user-456",
            TotalAmount = 100m,
            Currency = "TRY"
        };

        _paymentRepository.GetByOrderIdAsync(command.OrderId)
            .Returns(Task.FromResult<Payment?>(null));

        var gatewayResult = new PaymentGatewayResult
        {
            IsSuccess = true,
            TransactionId = "txn-789"
        };

        _paymentGateway.ProcessPaymentAsync(command.OrderId, command.TotalAmount, command.UserId)
            .Returns(Task.FromResult(gatewayResult));

        _paymentRepository.AddAsync(Arg.Any<Payment>())
            .Returns(args => Task.FromResult(args.Arg<Payment>()));

        _paymentRepository.UpdateAsync(Arg.Any<Payment>())
            .Returns(Task.CompletedTask);

        var expectedDto = new PaymentDto
        {
            OrderId = command.OrderId,
            UserId = command.UserId,
            Amount = command.TotalAmount,
            Status = PaymentStatus.Success
        };

        _mapper.Map<PaymentDto>(Arg.Any<Payment>())
            .Returns(expectedDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.OrderId.Should().Be(command.OrderId);
        result.Status.Should().Be(PaymentStatus.Success);

        await _paymentRepository.Received(1).AddAsync(Arg.Any<Payment>());
        await _paymentRepository.Received(1).UpdateAsync(Arg.Any<Payment>());
        await _paymentGateway.Received(1).ProcessPaymentAsync(command.OrderId, command.TotalAmount, command.UserId);
        await _publishEndpoint.Received(1).Publish<IPaymentSuccessEvent>(Arg.Any<object>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WithExistingOrder_ShouldReturnExistingPaymentWithoutProcessing()
    {
        // Arrange
        var command = new ProcessPaymentCommand
        {
            OrderId = "order-duplicate",
            UserId = "user-456",
            TotalAmount = 100m,
            Currency = "TRY"
        };

        var existingPayment = new Payment(command.OrderId, command.UserId, new Money(command.TotalAmount, command.Currency));
        existingPayment.MarkAsProcessing();
        existingPayment.MarkAsSuccess("existing-txn");

        _paymentRepository.GetByOrderIdAsync(command.OrderId)
            .Returns(Task.FromResult<Payment?>(existingPayment));

        var expectedDto = new PaymentDto
        {
            OrderId = command.OrderId,
            UserId = command.UserId,
            Status = PaymentStatus.Success
        };

        _mapper.Map<PaymentDto>(existingPayment)
            .Returns(expectedDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.OrderId.Should().Be(command.OrderId);

        await _paymentRepository.DidNotReceive().AddAsync(Arg.Any<Payment>());
        await _paymentGateway.DidNotReceive().ProcessPaymentAsync(Arg.Any<string>(), Arg.Any<decimal>(), Arg.Any<string>());
        await _publishEndpoint.DidNotReceive().Publish<IPaymentSuccessEvent>(Arg.Any<object>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_WhenPaymentFails_ShouldPublishFailureEvent()
    {
        // Arrange
        var command = new ProcessPaymentCommand
        {
            OrderId = "order-fail",
            UserId = "user-456",
            TotalAmount = 100m,
            Currency = "TRY"
        };

        _paymentRepository.GetByOrderIdAsync(command.OrderId)
            .Returns(Task.FromResult<Payment?>(null));

        var gatewayResult = new PaymentGatewayResult
        {
            IsSuccess = false,
            FailureReason = "Insufficient funds"
        };

        _paymentGateway.ProcessPaymentAsync(command.OrderId, command.TotalAmount, command.UserId)
            .Returns(Task.FromResult(gatewayResult));

        _paymentRepository.AddAsync(Arg.Any<Payment>())
            .Returns(args => Task.FromResult(args.Arg<Payment>()));

        _paymentRepository.UpdateAsync(Arg.Any<Payment>())
            .Returns(Task.CompletedTask);

        var expectedDto = new PaymentDto
        {
            OrderId = command.OrderId,
            UserId = command.UserId,
            Status = PaymentStatus.Failed
        };

        _mapper.Map<PaymentDto>(Arg.Any<Payment>())
            .Returns(expectedDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(PaymentStatus.Failed);

        await _paymentRepository.Received(1).AddAsync(Arg.Any<Payment>());
        await _paymentRepository.Received(1).UpdateAsync(Arg.Any<Payment>());
        await _paymentGateway.Received(1).ProcessPaymentAsync(command.OrderId, command.TotalAmount, command.UserId);
        await _publishEndpoint.Received(1).Publish<IPaymentFailedEvent>(Arg.Any<object>(), Arg.Any<CancellationToken>());
        await _publishEndpoint.DidNotReceive().Publish<IPaymentSuccessEvent>(Arg.Any<object>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldCreatePaymentWithCorrectAmount()
    {
        // Arrange
        var command = new ProcessPaymentCommand
        {
            OrderId = "order-amount-test",
            UserId = "user-456",
            TotalAmount = 250.75m,
            Currency = "USD"
        };

        Payment? capturedPayment = null;

        _paymentRepository.GetByOrderIdAsync(command.OrderId)
            .Returns(Task.FromResult<Payment?>(null));

        _paymentRepository.AddAsync(Arg.Any<Payment>())
            .Returns(args =>
            {
                capturedPayment = args.Arg<Payment>();
                return Task.FromResult(capturedPayment);
            });

        var gatewayResult = new PaymentGatewayResult { IsSuccess = true, TransactionId = "txn-123" };
        _paymentGateway.ProcessPaymentAsync(Arg.Any<string>(), Arg.Any<decimal>(), Arg.Any<string>())
            .Returns(Task.FromResult(gatewayResult));

        _paymentRepository.UpdateAsync(Arg.Any<Payment>())
            .Returns(Task.CompletedTask);

        _mapper.Map<PaymentDto>(Arg.Any<Payment>())
            .Returns(new PaymentDto { OrderId = command.OrderId });

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        capturedPayment.Should().NotBeNull();
        capturedPayment!.Amount.Amount.Should().Be(command.TotalAmount);
        capturedPayment.Amount.Currency.Should().Be(command.Currency);
    }

    [Fact]
    public async Task Handle_ShouldCallGatewayWithCorrectParameters()
    {
        // Arrange
        var command = new ProcessPaymentCommand
        {
            OrderId = "order-params-test",
            UserId = "user-params",
            TotalAmount = 500m,
            Currency = "TRY"
        };

        _paymentRepository.GetByOrderIdAsync(command.OrderId)
            .Returns(Task.FromResult<Payment?>(null));

        _paymentRepository.AddAsync(Arg.Any<Payment>())
            .Returns(args => Task.FromResult(args.Arg<Payment>()));

        var gatewayResult = new PaymentGatewayResult { IsSuccess = true, TransactionId = "txn-123" };
        _paymentGateway.ProcessPaymentAsync(Arg.Any<string>(), Arg.Any<decimal>(), Arg.Any<string>())
            .Returns(Task.FromResult(gatewayResult));

        _paymentRepository.UpdateAsync(Arg.Any<Payment>())
            .Returns(Task.CompletedTask);

        _mapper.Map<PaymentDto>(Arg.Any<Payment>())
            .Returns(new PaymentDto());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        await _paymentGateway.Received(1).ProcessPaymentAsync(
            command.OrderId,
            command.TotalAmount,
            command.UserId);
    }

    [Fact]
    public async Task Handle_WhenSuccessful_ShouldPublishEventWithCorrectData()
    {
        // Arrange
        var command = new ProcessPaymentCommand
        {
            OrderId = "order-event-test",
            UserId = "user-event",
            TotalAmount = 300m,
            Currency = "TRY"
        };

        object? publishedEvent = null;

        _paymentRepository.GetByOrderIdAsync(command.OrderId)
            .Returns(Task.FromResult<Payment?>(null));

        _paymentRepository.AddAsync(Arg.Any<Payment>())
            .Returns(args => Task.FromResult(args.Arg<Payment>()));

        var gatewayResult = new PaymentGatewayResult
        {
            IsSuccess = true,
            TransactionId = "txn-event-123"
        };

        _paymentGateway.ProcessPaymentAsync(Arg.Any<string>(), Arg.Any<decimal>(), Arg.Any<string>())
            .Returns(Task.FromResult(gatewayResult));

        _paymentRepository.UpdateAsync(Arg.Any<Payment>())
            .Returns(Task.CompletedTask);

        _publishEndpoint.Publish<IPaymentSuccessEvent>(Arg.Any<object>(), Arg.Any<CancellationToken>())
            .Returns(args =>
            {
                publishedEvent = args.Arg<object>();
                return Task.CompletedTask;
            });

        _mapper.Map<PaymentDto>(Arg.Any<Payment>())
            .Returns(new PaymentDto { OrderId = command.OrderId });

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        publishedEvent.Should().NotBeNull();
        await _publishEndpoint.Received(1).Publish<IPaymentSuccessEvent>(Arg.Any<object>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(50.0, "TRY")]
    [InlineData(100.5, "USD")]
    [InlineData(999.99, "EUR")]
    [InlineData(1500.75, "GBP")]
    public async Task Handle_WithDifferentAmountsAndCurrencies_ShouldProcessCorrectly(decimal amount, string currency)
    {
        // Arrange
        var command = new ProcessPaymentCommand
        {
            OrderId = $"order-{amount}-{currency}",
            UserId = "user-theory",
            TotalAmount = amount,
            Currency = currency
        };

        _paymentRepository.GetByOrderIdAsync(command.OrderId)
            .Returns(Task.FromResult<Payment?>(null));

        _paymentRepository.AddAsync(Arg.Any<Payment>())
            .Returns(args => Task.FromResult(args.Arg<Payment>()));

        var gatewayResult = new PaymentGatewayResult { IsSuccess = true, TransactionId = "txn-123" };
        _paymentGateway.ProcessPaymentAsync(Arg.Any<string>(), Arg.Any<decimal>(), Arg.Any<string>())
            .Returns(Task.FromResult(gatewayResult));

        _paymentRepository.UpdateAsync(Arg.Any<Payment>())
            .Returns(Task.CompletedTask);

        _mapper.Map<PaymentDto>(Arg.Any<Payment>())
            .Returns(new PaymentDto { OrderId = command.OrderId, Status = PaymentStatus.Success });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(PaymentStatus.Success);
        await _paymentGateway.Received(1).ProcessPaymentAsync(command.OrderId, amount, command.UserId);
    }

    [Fact]
    public async Task Handle_ShouldMarkPaymentAsProcessingBeforeGatewayCall()
    {
        // Arrange
        var command = new ProcessPaymentCommand
        {
            OrderId = "order-status-test",
            UserId = "user-status",
            TotalAmount = 100m,
            Currency = "TRY"
        };

        PaymentStatus? statusWhenAdded = null;

        _paymentRepository.GetByOrderIdAsync(command.OrderId)
            .Returns(Task.FromResult<Payment?>(null));

        _paymentRepository.AddAsync(Arg.Any<Payment>())
            .Returns(args =>
            {
                var payment = args.Arg<Payment>();
                statusWhenAdded = payment.Status;
                return Task.FromResult(payment);
            });

        var gatewayResult = new PaymentGatewayResult { IsSuccess = true, TransactionId = "txn-123" };
        _paymentGateway.ProcessPaymentAsync(Arg.Any<string>(), Arg.Any<decimal>(), Arg.Any<string>())
            .Returns(Task.FromResult(gatewayResult));

        _paymentRepository.UpdateAsync(Arg.Any<Payment>())
            .Returns(Task.CompletedTask);

        _mapper.Map<PaymentDto>(Arg.Any<Payment>())
            .Returns(new PaymentDto());

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        statusWhenAdded.Should().NotBeNull();
        statusWhenAdded.Should().Be(PaymentStatus.Processing);
    }
}
