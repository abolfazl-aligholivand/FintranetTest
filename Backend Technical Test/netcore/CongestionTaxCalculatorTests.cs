using congestion.calculator;
using System;
using Xunit;

public class CongestionTaxCalculatorTests
{
    [Fact]
    public void GetTax_NoPasses_ReturnsZero()
    {
        // Arrange
        var congestionTaxRule = new CongestionTaxRule();
        var calculator = new CongestionTaxCalculator(congestionTaxRule);
        var vehicle = new Car();

        // Act
        var result = calculator.GetTax(vehicle, new DateTime[] { });

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void GetTax_SinglePassWithinOneHour_ReturnsCorrectTax()
    {
        // Arrange
        var congestionTaxRule = new CongestionTaxRule();
        var calculator = new CongestionTaxCalculator(congestionTaxRule);
        var vehicle = new Car();
        var passes = new[]
        {
            new DateTime(2013, 1, 14, 6, 15, 0)
        };

        // Act
        var result = calculator.GetTax(vehicle, passes);

        // Assert
        Assert.Equal(13, result);
    }

    [Fact]
    public void GetTax_MultiplePassesWithinOneHour_ReturnsHighestTax()
    {
        // Arrange
        var congestionTaxRule = new CongestionTaxRule();
        var calculator = new CongestionTaxCalculator(congestionTaxRule);
        var vehicle = new Car();
        var passes = new[]
        {
            new DateTime(2013, 1, 14, 6, 15, 0),
            new DateTime(2013, 1, 14, 6, 45, 0),
            new DateTime(2013, 1, 14, 7, 15, 0)
        };

        // Act
        var result = calculator.GetTax(vehicle, passes);

        // Assert
        Assert.Equal(18, result);
    }

    [Fact]
    public void GetTax_PassesOverlappingDays_ReturnsCorrectTax()
    {
        // Arrange
        var congestionTaxRule = new CongestionTaxRule();
        var calculator = new CongestionTaxCalculator(congestionTaxRule);
        var vehicle = new Car();
        var passes = new[]
        {
            new DateTime(2013, 1, 14, 22, 0, 0),
            new DateTime(2013, 1, 15, 7, 30, 0)
        };

        // Act
        var result = calculator.GetTax(vehicle, passes);

        // Assert
        Assert.Equal(26, result);
    }

    [Fact]
    public void GetTax_TollFreeVehicle_ReturnsZero()
    {
        // Arrange
        var congestionTaxRule = new CongestionTaxRule();
        var calculator = new CongestionTaxCalculator(congestionTaxRule);
        var tollFreeVehicle = new Motorbike();
        var passes = new[]
        {
            new DateTime(2013, 1, 14, 8, 0, 0)
        };

        // Act
        var result = calculator.GetTax(tollFreeVehicle, passes);

        // Assert
        Assert.Equal(0, result);
    }
}

public class CongestionTaxRuleTests
{
    [Fact]
    public void IsTollFreeDate_Weekend_ReturnsTrue()
    {
        // Arrange
        var congestionTaxRule = new CongestionTaxRule();

        // Act
        var resultSaturday = congestionTaxRule.IsTollFreeDate(new DateTime(2013, 1, 12));
        var resultSunday = congestionTaxRule.IsTollFreeDate(new DateTime(2013, 1, 13));

        // Assert
        Assert.True(resultSaturday);
        Assert.True(resultSunday);
    }

    [Fact]
    public void IsTollFreeDate_PublicHoliday_ReturnsTrue()
    {
        // Arrange
        var congestionTaxRule = new CongestionTaxRule();

        // Act
        var result = congestionTaxRule.IsTollFreeDate(new DateTime(2013, 1, 1)); // New Year's Day

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsTollFreeVehicleType_TollFreeVehicle_ReturnsTrue()
    {
        // Arrange
        var congestionTaxRule = new CongestionTaxRule();

        // Act
        var result = congestionTaxRule.IsTollFreeVehicleType("Motorcycle");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsTollFreeVehicleType_NonTollFreeVehicle_ReturnsFalse()
    {
        // Arrange
        var congestionTaxRule = new CongestionTaxRule();

        // Act
        var result = congestionTaxRule.IsTollFreeVehicleType("Car");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void GetTollFee_ValidDateAndVehicle_ReturnsCorrectFee()
    {
        // Arrange
        var congestionTaxRule = new CongestionTaxRule();
        var validDate = new DateTime(2013, 1, 14, 7, 30, 0); // A weekday during chargeable hours
        var validVehicle = new Car();

        // Act
        var result = congestionTaxRule.GetTollFee(validDate, validVehicle);

        // Assert
        Assert.Equal(18, result);
    }
}
