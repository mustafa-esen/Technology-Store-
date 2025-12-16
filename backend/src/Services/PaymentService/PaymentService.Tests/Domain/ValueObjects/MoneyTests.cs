using FluentAssertions;
using PaymentService.Domain.ValueObjects;

namespace PaymentService.Tests.Domain.ValueObjects;

public class MoneyTests
{
    [Fact]
    public void Constructor_WithValidValues_ShouldCreateMoney()
    {
        // Arrange
        var amount = 100.50m;
        var currency = "TRY";

        // Act
        var money = new Money(amount, currency);

        // Assert
        money.Amount.Should().Be(amount);
        money.Currency.Should().Be(currency);
    }

    [Fact]
    public void Constructor_WithDefaultCurrency_ShouldUseTRY()
    {
        // Arrange
        var amount = 50m;

        // Act
        var money = new Money(amount);

        // Assert
        money.Amount.Should().Be(amount);
        money.Currency.Should().Be("TRY");
    }

    [Fact]
    public void Constructor_WithNegativeAmount_ShouldThrowArgumentException()
    {
        // Arrange
        var negativeAmount = -100m;

        // Act
        Action act = () => new Money(negativeAmount);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Amount cannot be negative*");
    }

    [Fact]
    public void Constructor_WithEmptyCurrency_ShouldThrowArgumentException()
    {
        // Arrange
        var amount = 100m;
        var emptyCurrency = string.Empty;

        // Act
        Action act = () => new Money(amount, emptyCurrency);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Currency cannot be empty*");
    }

    [Fact]
    public void Constructor_WithNullCurrency_ShouldThrowArgumentException()
    {
        // Arrange
        var amount = 100m;
        string? nullCurrency = null;

        // Act
        Action act = () => new Money(amount, nullCurrency!);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Currency cannot be empty*");
    }

    [Fact]
    public void AddOperator_WithSameCurrency_ShouldReturnSum()
    {
        // Arrange
        var money1 = new Money(100m, "TRY");
        var money2 = new Money(50m, "TRY");

        // Act
        var result = money1 + money2;

        // Assert
        result.Amount.Should().Be(150m);
        result.Currency.Should().Be("TRY");
    }

    [Fact]
    public void AddOperator_WithDifferentCurrencies_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var money1 = new Money(100m, "TRY");
        var money2 = new Money(50m, "USD");

        // Act
        Action act = () => { var result = money1 + money2; };

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot add money with different currencies*");
    }

    [Fact]
    public void SubtractOperator_WithSameCurrency_ShouldReturnDifference()
    {
        // Arrange
        var money1 = new Money(100m, "TRY");
        var money2 = new Money(30m, "TRY");

        // Act
        var result = money1 - money2;

        // Assert
        result.Amount.Should().Be(70m);
        result.Currency.Should().Be("TRY");
    }

    [Fact]
    public void SubtractOperator_WithDifferentCurrencies_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var money1 = new Money(100m, "TRY");
        var money2 = new Money(30m, "USD");

        // Act
        Action act = () => { var result = money1 - money2; };

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot subtract money with different currencies*");
    }

    [Fact]
    public void SubtractOperator_ResultingInNegative_ShouldThrowArgumentException()
    {
        // Arrange
        var money1 = new Money(50m, "TRY");
        var money2 = new Money(100m, "TRY");

        // Act
        Action act = () => { var result = money1 - money2; };

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*Amount cannot be negative*");
    }

    [Fact]
    public void Equals_WithSameAmountAndCurrency_ShouldReturnTrue()
    {
        // Arrange
        var money1 = new Money(100m, "TRY");
        var money2 = new Money(100m, "TRY");

        // Act
        var result = money1.Equals(money2);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentAmount_ShouldReturnFalse()
    {
        // Arrange
        var money1 = new Money(100m, "TRY");
        var money2 = new Money(50m, "TRY");

        // Act
        var result = money1.Equals(money2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_WithDifferentCurrency_ShouldReturnFalse()
    {
        // Arrange
        var money1 = new Money(100m, "TRY");
        var money2 = new Money(100m, "USD");

        // Act
        var result = money1.Equals(money2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_WithNull_ShouldReturnFalse()
    {
        // Arrange
        var money = new Money(100m, "TRY");

        // Act
        var result = money.Equals(null);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_WithSameValues_ShouldReturnSameHash()
    {
        // Arrange
        var money1 = new Money(100m, "TRY");
        var money2 = new Money(100m, "TRY");

        // Act
        var hash1 = money1.GetHashCode();
        var hash2 = money2.GetHashCode();

        // Assert
        hash1.Should().Be(hash2);
    }

    [Fact]
    public void GetHashCode_WithDifferentValues_ShouldReturnDifferentHash()
    {
        // Arrange
        var money1 = new Money(100m, "TRY");
        var money2 = new Money(50m, "TRY");

        // Act
        var hash1 = money1.GetHashCode();
        var hash2 = money2.GetHashCode();

        // Assert
        hash1.Should().NotBe(hash2);
    }

    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        // Arrange
        var money = new Money(1234.56m, "TRY");

        // Act
        var result = money.ToString();

        // Assert
        result.Should().Be("1.234,56 TRY"); // Turkish number format
    }

    [Theory]
    [InlineData(0, "TRY", "0,00 TRY")]
    [InlineData(1, "USD", "1,00 USD")]
    [InlineData(999.99, "EUR", "999,99 EUR")]
    [InlineData(1000000, "TRY", "1.000.000,00 TRY")]
    public void ToString_WithVariousAmounts_ShouldReturnCorrectFormat(decimal amount, string currency, string expected)
    {
        // Arrange
        var money = new Money(amount, currency);

        // Act
        var result = money.ToString();

        // Assert
        result.Should().Be(expected);
    }
}
