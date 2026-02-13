using System;
using Xunit;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Tests;

/// <summary>
/// Unit tests for individual incentive calculators.
/// Tests validation logic and exact calculation formulas for each calculator type.
/// </summary>
public class IncentiveCalculatorTests
{
    #region FixedCashAmountCalculator Tests

    [Fact]
    public void FixedCashAmountCalculator_WithValidInputs_ShouldReturnValidCalculation()
    {
        // Arrange
        var calculator = new FixedCashAmountCalculator();
        var rebate = new Rebate
        {
            Identifier = "TEST001",
            Incentive = IncentiveType.FixedCashAmount,
            Amount = 50.75m
        };
        var product = new Product
        {
            Identifier = "PROD001",
            SupportedIncentives = SupportedIncentiveType.FixedCashAmount
        };
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "TEST001",
            ProductIdentifier = "PROD001",
            Volume = 100m
        };

        // Act
        var result = calculator.Calculate(rebate, product, request, out bool isValid);

        // Assert
        Assert.True(isValid);
        Assert.Equal(50.75m, result);
    }

    [Fact]
    public void FixedCashAmountCalculator_WithZeroAmount_ShouldReturnInvalid()
    {
        // Arrange
        var calculator = new FixedCashAmountCalculator();
        var rebate = new Rebate
        {
            Identifier = "TEST001",
            Incentive = IncentiveType.FixedCashAmount,
            Amount = 0m // Invalid amount
        };
        var product = new Product
        {
            Identifier = "PROD001",
            SupportedIncentives = SupportedIncentiveType.FixedCashAmount
        };
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "TEST001",
            ProductIdentifier = "PROD001",
            Volume = 100m
        };

        // Act
        var result = calculator.Calculate(rebate, product, request, out bool isValid);

        // Assert
        Assert.False(isValid);
        Assert.Equal(0m, result);
    }

    [Fact]
    public void FixedCashAmountCalculator_WithUnsupportedProduct_ShouldReturnInvalid()
    {
        // Arrange
        var calculator = new FixedCashAmountCalculator();
        var rebate = new Rebate
        {
            Identifier = "TEST001",
            Incentive = IncentiveType.FixedCashAmount,
            Amount = 50.00m
        };
        var product = new Product
        {
            Identifier = "PROD001",
            SupportedIncentives = SupportedIncentiveType.FixedRateRebate // Wrong type
        };
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "TEST001",
            ProductIdentifier = "PROD001",
            Volume = 100m
        };

        // Act
        var result = calculator.Calculate(rebate, product, request, out bool isValid);

        // Assert
        Assert.False(isValid);
        Assert.Equal(0m, result);
    }

    [Fact]
    public void FixedCashAmountCalculator_WithNullRebate_ShouldReturnInvalid()
    {
        // Arrange
        var calculator = new FixedCashAmountCalculator();
        var product = new Product
        {
            Identifier = "PROD001",
            SupportedIncentives = SupportedIncentiveType.FixedCashAmount
        };
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "TEST001",
            ProductIdentifier = "PROD001",
            Volume = 100m
        };

        // Act
        var result = calculator.Calculate(null, product, request, out bool isValid);

        // Assert
        Assert.False(isValid);
        Assert.Equal(0m, result);
    }

    #endregion

    #region FixedRateRebateCalculator Tests

    [Fact]
    public void FixedRateRebateCalculator_WithValidInputs_ShouldReturnValidCalculation()
    {
        // Arrange
        var calculator = new FixedRateRebateCalculator();
        var rebate = new Rebate
        {
            Identifier = "TEST002",
            Incentive = IncentiveType.FixedRateRebate,
            Percentage = 0.15m // 15%
        };
        var product = new Product
        {
            Identifier = "PROD002",
            Price = 200.00m,
            SupportedIncentives = SupportedIncentiveType.FixedRateRebate
        };
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "TEST002",
            ProductIdentifier = "PROD002",
            Volume = 10m
        };

        // Act
        var result = calculator.Calculate(rebate, product, request, out bool isValid);

        // Assert
        Assert.True(isValid);
        Assert.Equal(300.00m, result); // 200 * 0.15 * 10 = 300
    }

    [Fact]
    public void FixedRateRebateCalculator_WithZeroPercentage_ShouldReturnInvalid()
    {
        // Arrange
        var calculator = new FixedRateRebateCalculator();
        var rebate = new Rebate
        {
            Identifier = "TEST002",
            Incentive = IncentiveType.FixedRateRebate,
            Percentage = 0m // Invalid percentage
        };
        var product = new Product
        {
            Identifier = "PROD002",
            Price = 200.00m,
            SupportedIncentives = SupportedIncentiveType.FixedRateRebate
        };
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "TEST002",
            ProductIdentifier = "PROD002",
            Volume = 10m
        };

        // Act
        var result = calculator.Calculate(rebate, product, request, out bool isValid);

        // Assert
        Assert.False(isValid);
        Assert.Equal(0m, result);
    }

    [Fact]
    public void FixedRateRebateCalculator_WithNullProduct_ShouldReturnInvalid()
    {
        // Arrange
        var calculator = new FixedRateRebateCalculator();
        var rebate = new Rebate
        {
            Identifier = "TEST002",
            Incentive = IncentiveType.FixedRateRebate,
            Percentage = 0.15m
        };
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "TEST002",
            ProductIdentifier = "PROD002",
            Volume = 10m
        };

        // Act
        var result = calculator.Calculate(rebate, null, request, out bool isValid);

        // Assert
        Assert.False(isValid);
        Assert.Equal(0m, result);
    }

    [Fact]
    public void FixedRateRebateCalculator_WithZeroVolume_ShouldReturnInvalid()
    {
        // Arrange
        var calculator = new FixedRateRebateCalculator();
        var rebate = new Rebate
        {
            Identifier = "TEST002",
            Incentive = IncentiveType.FixedRateRebate,
            Percentage = 0.15m
        };
        var product = new Product
        {
            Identifier = "PROD002",
            Price = 200.00m,
            SupportedIncentives = SupportedIncentiveType.FixedRateRebate
        };
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "TEST002",
            ProductIdentifier = "PROD002",
            Volume = 0m // Invalid volume
        };

        // Act
        var result = calculator.Calculate(rebate, product, request, out bool isValid);

        // Assert
        Assert.False(isValid);
        Assert.Equal(0m, result);
    }

    #endregion

    #region AmountPerUomCalculator Tests

    [Fact]
    public void AmountPerUomCalculator_WithValidInputs_ShouldReturnValidCalculation()
    {
        // Arrange
        var calculator = new AmountPerUomCalculator();
        var rebate = new Rebate
        {
            Identifier = "TEST003",
            Incentive = IncentiveType.AmountPerUom,
            Amount = 5.25m
        };
        var product = new Product
        {
            Identifier = "PROD003",
            SupportedIncentives = SupportedIncentiveType.AmountPerUom
        };
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "TEST003",
            ProductIdentifier = "PROD003",
            Volume = 8m
        };

        // Act
        var result = calculator.Calculate(rebate, product, request, out bool isValid);

        // Assert
        Assert.True(isValid);
        Assert.Equal(42.00m, result); // 5.25 * 8 = 42.00
    }

    [Fact]
    public void AmountPerUomCalculator_WithZeroAmount_ShouldReturnInvalid()
    {
        // Arrange
        var calculator = new AmountPerUomCalculator();
        var rebate = new Rebate
        {
            Identifier = "TEST003",
            Incentive = IncentiveType.AmountPerUom,
            Amount = 0m // Invalid amount
        };
        var product = new Product
        {
            Identifier = "PROD003",
            SupportedIncentives = SupportedIncentiveType.AmountPerUom
        };
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "TEST003",
            ProductIdentifier = "PROD003",
            Volume = 8m
        };

        // Act
        var result = calculator.Calculate(rebate, product, request, out bool isValid);

        // Assert
        Assert.False(isValid);
        Assert.Equal(0m, result);
    }

    [Fact]
    public void AmountPerUomCalculator_WithUnsupportedProduct_ShouldReturnInvalid()
    {
        // Arrange
        var calculator = new AmountPerUomCalculator();
        var rebate = new Rebate
        {
            Identifier = "TEST003",
            Incentive = IncentiveType.AmountPerUom,
            Amount = 5.25m
        };
        var product = new Product
        {
            Identifier = "PROD003",
            SupportedIncentives = SupportedIncentiveType.FixedCashAmount // Wrong type
        };
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "TEST003",
            ProductIdentifier = "PROD003",
            Volume = 8m
        };

        // Act
        var result = calculator.Calculate(rebate, product, request, out bool isValid);

        // Assert
        Assert.False(isValid);
        Assert.Equal(0m, result);
    }

    [Fact]
    public void AmountPerUomCalculator_WithNegativeVolume_ShouldReturnInvalid()
    {
        // Arrange
        var calculator = new AmountPerUomCalculator();
        var rebate = new Rebate
        {
            Identifier = "TEST003",
            Incentive = IncentiveType.AmountPerUom,
            Amount = 5.25m
        };
        var product = new Product
        {
            Identifier = "PROD003",
            SupportedIncentives = SupportedIncentiveType.AmountPerUom
        };
        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "TEST003",
            ProductIdentifier = "PROD003",
            Volume = -5m // Invalid volume
        };

        // Act
        var result = calculator.Calculate(rebate, product, request, out bool isValid);

        // Assert
        Assert.False(isValid);
        Assert.Equal(0m, result);
    }

    #endregion

    #region IncentiveType Property Tests

    [Fact]
    public void FixedCashAmountCalculator_IncentiveType_ShouldReturnCorrectType()
    {
        // Arrange
        var calculator = new FixedCashAmountCalculator();

        // Act & Assert
        Assert.Equal(IncentiveType.FixedCashAmount, calculator.IncentiveType);
    }

    [Fact]
    public void FixedRateRebateCalculator_IncentiveType_ShouldReturnCorrectType()
    {
        // Arrange
        var calculator = new FixedRateRebateCalculator();

        // Act & Assert
        Assert.Equal(IncentiveType.FixedRateRebate, calculator.IncentiveType);
    }

    [Fact]
    public void AmountPerUomCalculator_IncentiveType_ShouldReturnCorrectType()
    {
        // Arrange
        var calculator = new AmountPerUomCalculator();

        // Act & Assert
        Assert.Equal(IncentiveType.AmountPerUom, calculator.IncentiveType);
    }

    #endregion
}