// SMARTWYRE DEVELOPER TEST REFACTOR
// REFACTORED: The RebateService now follows SOLID principles:
// - Single Responsibility: Only orchestrates the rebate calculation flow
// - Open/Closed: New incentive types can be added without modifying this class
// - Dependency Inversion: Depends on abstractions, not concretions
// - Strategy Pattern: Delegates calculation logic to incentive-specific calculators

using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services;

public class RebateService : IRebateService
{
    private readonly IRebateDataStore _rebateDataStore;
    private readonly IProductDataStore _productDataStore;
    private readonly IIncentiveCalculatorResolver _calculatorResolver;

    public RebateService(
        IRebateDataStore rebateDataStore = null,
        IProductDataStore productDataStore = null,
        IIncentiveCalculatorResolver calculatorResolver = null)
    {
        _rebateDataStore = rebateDataStore ?? new RebateDataStore();
        _productDataStore = productDataStore ?? new ProductDataStore();
        _calculatorResolver = calculatorResolver ?? new IncentiveCalculatorResolver();
    }

    public CalculateRebateResult Calculate(CalculateRebateRequest request)
    {
        // Load required data
        Rebate rebate = _rebateDataStore.GetRebate(request.RebateIdentifier);
        Product product = _productDataStore.GetProduct(request.ProductIdentifier);

        var result = new CalculateRebateResult();

        // Basic validation
        if (rebate == null)
        {
            result.Success = false;
            return result;
        }

        // Get the appropriate calculator for this incentive type
        var calculator = _calculatorResolver.GetCalculator(rebate.Incentive);
        
        // Delegate calculation to the specific calculator
        var rebateAmount = calculator.Calculate(rebate, product, request, out bool isValid);
        
        result.Success = isValid;

        // Store result only if calculation was successful
        if (result.Success)
        {
            _rebateDataStore.StoreCalculationResult(rebate, rebateAmount);
        }

        return result;
    }
}
