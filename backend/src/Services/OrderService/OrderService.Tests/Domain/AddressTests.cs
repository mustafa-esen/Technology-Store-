using FluentAssertions;
using OrderService.Domain.ValueObjects;

namespace OrderService.Tests.Domain;

public class AddressTests
{
    [Fact]
    public void Constructor_ShouldCreateAddress_WithValidData()
    {
        // Arrange
        var street = "123 Main St";
        var city = "New York";
        var state = "NY";
        var zipCode = "10001";
        var country = "USA";

        // Act
        var address = new Address(street, city, state, zipCode, country);

        // Assert
        address.Street.Should().Be(street);
        address.City.Should().Be(city);
        address.State.Should().Be(state);
        address.ZipCode.Should().Be(zipCode);
        address.Country.Should().Be(country);
    }

    [Fact]
    public void GetFullAddress_ShouldReturnFormattedAddress()
    {
        // Arrange
        var address = new Address("123 Main St", "New York", "NY", "10001", "USA");

        // Act
        var fullAddress = address.GetFullAddress();

        // Assert
        fullAddress.Should().Be("123 Main St, New York, NY 10001, USA");
    }

    [Fact]
    public void DefaultConstructor_ShouldCreateEmptyAddress()
    {
        // Act
        var address = new Address();

        // Assert
        address.Street.Should().BeEmpty();
        address.City.Should().BeEmpty();
        address.State.Should().BeEmpty();
        address.ZipCode.Should().BeEmpty();
        address.Country.Should().BeEmpty();
    }
}
