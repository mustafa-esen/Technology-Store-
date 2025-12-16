using AutoMapper;
using FluentAssertions;
using NSubstitute;
using PaymentService.Application.DTOs;
using PaymentService.Application.Features.Payments.Queries.GetPaymentById;
using PaymentService.Application.Interfaces;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Enums;
using PaymentService.Domain.ValueObjects;

namespace PaymentService.Tests.Application.Queries;

public class GetPaymentByIdQueryHandlerTests
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IMapper _mapper;
    private readonly GetPaymentByIdQueryHandler _handler;

    public GetPaymentByIdQueryHandlerTests()
    {
        _paymentRepository = Substitute.For<IPaymentRepository>();
        _mapper = Substitute.For<IMapper>();

        _handler = new GetPaymentByIdQueryHandler(_paymentRepository, _mapper);
    }

    [Fact]
    public async Task Handle_WhenPaymentExists_ShouldReturnPaymentDto()
    {
        // Arrange
        var paymentId = Guid.NewGuid();
        var query = new GetPaymentByIdQuery { Id = paymentId };

        var payment = new Payment("order-123", "user-456", new Money(100m, "TRY"));

        _paymentRepository.GetByIdAsync(paymentId)
            .Returns(Task.FromResult<Payment?>(payment));

        var expectedDto = new PaymentDto
        {
            Id = paymentId,
            OrderId = "order-123",
            UserId = "user-456",
            Amount = 100m,
            Currency = "TRY",
            Status = PaymentStatus.Pending
        };

        _mapper.Map<PaymentDto>(payment)
            .Returns(expectedDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(expectedDto);
        result!.OrderId.Should().Be("order-123");
        result.UserId.Should().Be("user-456");
        result.Amount.Should().Be(100m);

        await _paymentRepository.Received(1).GetByIdAsync(paymentId);
        _mapper.Received(1).Map<PaymentDto>(payment);
    }

    [Fact]
    public async Task Handle_WhenPaymentDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var paymentId = Guid.NewGuid();
        var query = new GetPaymentByIdQuery { Id = paymentId };

        _paymentRepository.GetByIdAsync(paymentId)
            .Returns(Task.FromResult<Payment?>(null));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();

        await _paymentRepository.Received(1).GetByIdAsync(paymentId);
        _mapper.DidNotReceive().Map<PaymentDto>(Arg.Any<Payment>());
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryWithCorrectId()
    {
        // Arrange
        var paymentId = Guid.NewGuid();
        var query = new GetPaymentByIdQuery { Id = paymentId };

        _paymentRepository.GetByIdAsync(paymentId)
            .Returns(Task.FromResult<Payment?>(null));

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        await _paymentRepository.Received(1).GetByIdAsync(paymentId);
    }

    [Fact]
    public async Task Handle_ShouldUseMapperToConvertToDto()
    {
        // Arrange
        var paymentId = Guid.NewGuid();
        var query = new GetPaymentByIdQuery { Id = paymentId };

        var payment = new Payment("order-789", "user-789", new Money(250m, "USD"));

        _paymentRepository.GetByIdAsync(paymentId)
            .Returns(Task.FromResult<Payment?>(payment));

        _mapper.Map<PaymentDto>(payment)
            .Returns(new PaymentDto { Id = paymentId, OrderId = "order-789" });

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _mapper.Received(1).Map<PaymentDto>(payment);
    }

    [Theory]
    [InlineData(PaymentStatus.Pending)]
    [InlineData(PaymentStatus.Processing)]
    [InlineData(PaymentStatus.Success)]
    [InlineData(PaymentStatus.Failed)]
    [InlineData(PaymentStatus.Refunded)]
    public async Task Handle_WithDifferentPaymentStatuses_ShouldReturnCorrectDto(PaymentStatus status)
    {
        // Arrange
        var paymentId = Guid.NewGuid();
        var query = new GetPaymentByIdQuery { Id = paymentId };

        var payment = new Payment("order-status", "user-status", new Money(100m, "TRY"));

        _paymentRepository.GetByIdAsync(paymentId)
            .Returns(Task.FromResult<Payment?>(payment));

        var expectedDto = new PaymentDto
        {
            Id = paymentId,
            Status = status
        };

        _mapper.Map<PaymentDto>(payment)
            .Returns(expectedDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Status.Should().Be(status);
    }
}
