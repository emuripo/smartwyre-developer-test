using System;
using Xunit;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Types;
using Smartwyre.DeveloperTest.Data;

namespace Smartwyre.DeveloperTest.Tests;

/// <summary>
/// Unit tests for RebateService after SOLID principles refactoring.
/// Tests verify correct calculator selection, validation, and no persistence on invalid requests.
/// </summary>
public class RebateServiceTests
{
    [Fact]
    public void Calculate_WithValidFixedCashAmount_ShouldReturnSuccess()
    {
        // Arrange
        var rebateDataStore = new MockRebateDataStore();
        var productDataStore = new MockProductDataStore();
        var service = new RebateService(rebateDataStore, productDataStore);

        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "REBATE001",
            ProductIdentifier = "PROD001", 
            Volume = 100
        };

        rebateDataStore.SetupRebate(new Rebate
        {
            Identifier = "REBATE001",
            Incentive = IncentiveType.FixedCashAmount,
            Amount = 50.00m
        });

        productDataStore.SetupProduct(new Product
        {
            Identifier = "PROD001",
            SupportedIncentives = SupportedIncentiveType.FixedCashAmount
        });

        // Act
        var result = service.Calculate(request);

        // Assert
        Assert.True(result.Success);
        Assert.True(rebateDataStore.WasStoreCalculationResultCalled);
    }

    [Fact]
    public void Calculate_WithValidFixedRateRebate_ShouldReturnSuccess()
    {
        // Arrange
        var rebateDataStore = new MockRebateDataStore();
        var productDataStore = new MockProductDataStore();
        var service = new RebateService(rebateDataStore, productDataStore);

        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "REBATE002",
            ProductIdentifier = "PROD002",
            Volume = 10
        };

        rebateDataStore.SetupRebate(new Rebate
        {
            Identifier = "REBATE002",
            Incentive = IncentiveType.FixedRateRebate,
            Percentage = 0.1m
        });

        productDataStore.SetupProduct(new Product
        {
            Identifier = "PROD002",
            Price = 100.00m,
            SupportedIncentives = SupportedIncentiveType.FixedRateRebate
        });

        // Act
        var result = service.Calculate(request);

        // Assert
        Assert.True(result.Success);
        Assert.True(rebateDataStore.WasStoreCalculationResultCalled);
    }

    [Fact]
    public void Calculate_WithValidAmountPerUom_ShouldReturnSuccess()
    {
        // Arrange
        var rebateDataStore = new MockRebateDataStore();
        var productDataStore = new MockProductDataStore();
        var service = new RebateService(rebateDataStore, productDataStore);

        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "REBATE003",
            ProductIdentifier = "PROD003",
            Volume = 5
        };

        rebateDataStore.SetupRebate(new Rebate
        {
            Identifier = "REBATE003",
            Incentive = IncentiveType.AmountPerUom,
            Amount = 10.00m
        });

        productDataStore.SetupProduct(new Product
        {
            Identifier = "PROD003",
            SupportedIncentives = SupportedIncentiveType.AmountPerUom
        });

        // Act
        var result = service.Calculate(request);

        // Assert
        Assert.True(result.Success);
        Assert.True(rebateDataStore.WasStoreCalculationResultCalled);
    }

    [Fact]
    public void Calculate_WithNullRebate_ShouldReturnFailure()
    {
        // Arrange
        var rebateDataStore = new MockRebateDataStore();
        var productDataStore = new MockProductDataStore();
        var service = new RebateService(rebateDataStore, productDataStore);

        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "INVALID",
            ProductIdentifier = "PROD001",
            Volume = 100
        };

        // rebateDataStore returns null for unknown identifiers

        // Act
        var result = service.Calculate(request);

        // Assert
        Assert.False(result.Success);
        Assert.False(rebateDataStore.WasStoreCalculationResultCalled);
    }

    [Fact]
    public void Calculate_WithUnsupportedIncentiveType_ShouldReturnFailure()
    {
        // Arrange
        var rebateDataStore = new MockRebateDataStore();
        var productDataStore = new MockProductDataStore();
        var service = new RebateService(rebateDataStore, productDataStore);

        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "REBATE004",
            ProductIdentifier = "PROD004",
            Volume = 100
        };

        rebateDataStore.SetupRebate(new Rebate
        {
            Identifier = "REBATE004",
            Incentive = IncentiveType.FixedCashAmount,
            Amount = 50.00m
        });

        productDataStore.SetupProduct(new Product
        {
            Identifier = "PROD004",
            SupportedIncentives = SupportedIncentiveType.FixedRateRebate // Different type
        });

        // Act
        var result = service.Calculate(request);

        // Assert
        Assert.False(result.Success);
        Assert.False(rebateDataStore.WasStoreCalculationResultCalled);
    }

    [Fact]
    public void Calculate_WithInvalidAmounts_ShouldReturnFailure()
    {
        // Arrange
        var rebateDataStore = new MockRebateDataStore();
        var productDataStore = new MockProductDataStore();
        var service = new RebateService(rebateDataStore, productDataStore);

        var request = new CalculateRebateRequest
        {
            RebateIdentifier = "REBATE005",
            ProductIdentifier = "PROD005",
            Volume = 100
        };

        rebateDataStore.SetupRebate(new Rebate
        {
            Identifier = "REBATE005",
            Incentive = IncentiveType.FixedCashAmount,
            Amount = 0m // Invalid amount
        });

        productDataStore.SetupProduct(new Product
        {
            Identifier = "PROD005",
            SupportedIncentives = SupportedIncentiveType.FixedCashAmount
        });

        // Act
        var result = service.Calculate(request);

        // Assert
        Assert.False(result.Success);
        Assert.False(rebateDataStore.WasStoreCalculationResultCalled);
    }

    [Theory]
    [InlineData(IncentiveType.FixedCashAmount, typeof(FixedCashAmountCalculator))]
    [InlineData(IncentiveType.FixedRateRebate, typeof(FixedRateRebateCalculator))]
    [InlineData(IncentiveType.AmountPerUom, typeof(AmountPerUomCalculator))]
    public void IncentiveCalculatorResolver_ShouldReturnCorrectCalculatorType(
        IncentiveType incentiveType, Type expectedCalculatorType)
    {
        // Arrange
        var resolver = new IncentiveCalculatorResolver();

        // Act
        var calculator = resolver.GetCalculator(incentiveType);

        // Assert
        Assert.Equal(expectedCalculatorType, calculator.GetType());
        Assert.Equal(incentiveType, calculator.IncentiveType);
    }
}

/// <summary>
/// Mock implementation of IRebateDataStore for testing
/// </summary>
public class MockRebateDataStore : IRebateDataStore
{
    private Rebate _rebate;
    public bool WasStoreCalculationResultCalled { get; private set; }

    public void SetupRebate(Rebate rebate)
    {
        _rebate = rebate;
    }

    public Rebate GetRebate(string rebateIdentifier)
    {
        return _rebate?.Identifier == rebateIdentifier ? _rebate : null;
    }

    public void StoreCalculationResult(Rebate rebate, decimal rebateAmount)
    {
        WasStoreCalculationResultCalled = true;
    }
}

/// <summary>
/// Mock implementation of IProductDataStore for testing
/// </summary>
public class MockProductDataStore : IProductDataStore
{
    private Product _product;

    public void SetupProduct(Product product)
    {
        _product = product;
    }

    public Product GetProduct(string productIdentifier)
    {
        return _product?.Identifier == productIdentifier ? _product : null;
    }
}
