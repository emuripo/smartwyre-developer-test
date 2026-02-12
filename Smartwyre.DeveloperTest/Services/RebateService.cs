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
            result.Message = "Rebate not found";
            result.Amount = 0;
            return result;
        }

        // Get the appropriate calculator for this incentive type
        var calculator = _calculatorResolver.GetCalculator(rebate.Incentive);
        
        // Delegate calculation to the specific calculator
        var rebateAmount = calculator.Calculate(rebate, product, request, out bool isValid);
        
        result.Success = isValid;
        result.Amount = rebateAmount;

        if (isValid)
        {
            result.Message = "Rebate calculated successfully";
            // Store result only if calculation was successful
            _rebateDataStore.StoreCalculationResult(rebate, rebateAmount);
        }
        else
        {
            result.Message = "Invalid rebate calculation - check product compatibility and values";
        }

        return result;
    }
}
