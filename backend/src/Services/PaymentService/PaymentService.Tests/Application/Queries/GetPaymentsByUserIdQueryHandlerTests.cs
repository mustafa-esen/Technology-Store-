using AutoMapper;
using FluentAssertions;
using NSubstitute;
using PaymentService.Application.DTOs;
using PaymentService.Application.Features.Payments.Queries.GetPaymentsByUserId;
using PaymentService.Application.Interfaces;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Enums;
using PaymentService.Domain.ValueObjects;

namespace PaymentService.Tests.Application.Queries;

public class GetPaymentsByUserIdQueryHandlerTests
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IMapper _mapper;
    private readonly GetPaymentsByUserIdQueryHandler _handler;

    public GetPaymentsByUserIdQueryHandlerTests()
    {
        _paymentRepository = Substitute.For<IPaymentRepository>();
        _mapper = Substitute.For<IMapper>();

        _handler = new GetPaymentsByUserIdQueryHandler(_paymentRepository, _mapper);
    }

    [Fact]
    public async Task Handle_WhenUserHasPayments_ShouldReturnListOfPaymentDtos()
    {
        // Arrange
        var userId = "user-123";
        var query = new GetPaymentsByUserIdQuery { UserId = userId };

        var payments = new List<Payment>
        {
            new Payment("order-1", userId, new Money(100m, "TRY")),
            new Payment("order-2", userId, new Money(200m, "TRY")),
            new Payment("order-3", userId, new Money(150m, "USD"))
        };

        _paymentRepository.GetByUserIdAsync(userId)
            .Returns(Task.FromResult(payments));

        var expectedDtos = new List<PaymentDto>
        {
            new PaymentDto { OrderId = "order-1", UserId = userId, Amount = 100m },
            new PaymentDto { OrderId = "order-2", UserId = userId, Amount = 200m },
            new PaymentDto { OrderId = "order-3", UserId = userId, Amount = 150m }
        };

        _mapper.Map<List<PaymentDto>>(payments)
            .Returns(expectedDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().BeEquivalentTo(expectedDtos);

        await _paymentRepository.Received(1).GetByUserIdAsync(userId);
        _mapper.Received(1).Map<List<PaymentDto>>(payments);
    }

    [Fact]
    public async Task Handle_WhenUserHasNoPayments_ShouldReturnEmptyList()
    {
        // Arrange
        var userId = "user-no-payments";
        var query = new GetPaymentsByUserIdQuery { UserId = userId };

        var emptyPayments = new List<Payment>();

        _paymentRepository.GetByUserIdAsync(userId)
            .Returns(Task.FromResult(emptyPayments));

        _mapper.Map<List<PaymentDto>>(emptyPayments)
            .Returns(new List<PaymentDto>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        await _paymentRepository.Received(1).GetByUserIdAsync(userId);
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryWithCorrectUserId()
    {
        // Arrange
        var userId = "user-specific";
        var query = new GetPaymentsByUserIdQuery { UserId = userId };

        _paymentRepository.GetByUserIdAsync(userId)
            .Returns(Task.FromResult(new List<Payment>()));

        _mapper.Map<List<PaymentDto>>(Arg.Any<List<Payment>>())
            .Returns(new List<PaymentDto>());

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        await _paymentRepository.Received(1).GetByUserIdAsync(userId);
    }

    [Fact]
    public async Task Handle_ShouldUseMapperToConvertListToDto()
    {
        // Arrange
        var userId = "user-mapper-test";
        var query = new GetPaymentsByUserIdQuery { UserId = userId };

        var payments = new List<Payment>
        {
            new Payment("order-1", userId, new Money(100m, "TRY"))
        };

        _paymentRepository.GetByUserIdAsync(userId)
            .Returns(Task.FromResult(payments));

        _mapper.Map<List<PaymentDto>>(payments)
            .Returns(new List<PaymentDto> { new PaymentDto { OrderId = "order-1" } });

        // Act
        await _handler.Handle(query, CancellationToken.None);

        // Assert
        _mapper.Received(1).Map<List<PaymentDto>>(payments);
    }

    [Fact]
    public async Task Handle_WithMultiplePaymentStatuses_ShouldReturnAll()
    {
        // Arrange
        var userId = "user-multi-status";
        var query = new GetPaymentsByUserIdQuery { UserId = userId };

        var payment1 = new Payment("order-pending", userId, new Money(100m, "TRY"));

        var payment2 = new Payment("order-success", userId, new Money(200m, "TRY"));
        payment2.MarkAsProcessing();
        payment2.MarkAsSuccess("txn-123");

        var payment3 = new Payment("order-failed", userId, new Money(150m, "TRY"));
        payment3.MarkAsProcessing();
        payment3.MarkAsFailed("Insufficient funds");

        var payments = new List<Payment> { payment1, payment2, payment3 };

        _paymentRepository.GetByUserIdAsync(userId)
            .Returns(Task.FromResult(payments));

        var expectedDtos = new List<PaymentDto>
        {
            new PaymentDto { OrderId = "order-pending", Status = PaymentStatus.Pending },
            new PaymentDto { OrderId = "order-success", Status = PaymentStatus.Success },
            new PaymentDto { OrderId = "order-failed", Status = PaymentStatus.Failed }
        };

        _mapper.Map<List<PaymentDto>>(payments)
            .Returns(expectedDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().Contain(dto => dto.Status == PaymentStatus.Pending);
        result.Should().Contain(dto => dto.Status == PaymentStatus.Success);
        result.Should().Contain(dto => dto.Status == PaymentStatus.Failed);
    }

    [Fact]
    public async Task Handle_WithDifferentCurrencies_ShouldReturnAll()
    {
        // Arrange
        var userId = "user-multi-currency";
        var query = new GetPaymentsByUserIdQuery { UserId = userId };

        var payments = new List<Payment>
        {
            new Payment("order-try", userId, new Money(100m, "TRY")),
            new Payment("order-usd", userId, new Money(50m, "USD")),
            new Payment("order-eur", userId, new Money(75m, "EUR"))
        };

        _paymentRepository.GetByUserIdAsync(userId)
            .Returns(Task.FromResult(payments));

        var expectedDtos = new List<PaymentDto>
        {
            new PaymentDto { OrderId = "order-try", Currency = "TRY" },
            new PaymentDto { OrderId = "order-usd", Currency = "USD" },
            new PaymentDto { OrderId = "order-eur", Currency = "EUR" }
        };

        _mapper.Map<List<PaymentDto>>(payments)
            .Returns(expectedDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().Contain(dto => dto.Currency == "TRY");
        result.Should().Contain(dto => dto.Currency == "USD");
        result.Should().Contain(dto => dto.Currency == "EUR");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(50)]
    public async Task Handle_WithVariousPaymentCounts_ShouldReturnCorrectCount(int paymentCount)
    {
        // Arrange
        var userId = $"user-count-{paymentCount}";
        var query = new GetPaymentsByUserIdQuery { UserId = userId };

        var payments = new List<Payment>();
        for (int i = 0; i < paymentCount; i++)
        {
            payments.Add(new Payment($"order-{i}", userId, new Money(100m, "TRY")));
        }

        _paymentRepository.GetByUserIdAsync(userId)
            .Returns(Task.FromResult(payments));

        var dtos = payments.Select(p => new PaymentDto { OrderId = p.OrderId }).ToList();
        _mapper.Map<List<PaymentDto>>(payments)
            .Returns(dtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(paymentCount);
    }
}
