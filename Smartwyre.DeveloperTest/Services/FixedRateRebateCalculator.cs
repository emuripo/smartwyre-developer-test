using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services;

public class FixedRateRebateCalculator : IIncentiveCalculator
{
    public IncentiveType IncentiveType => IncentiveType.FixedRateRebate;

    public decimal Calculate(Rebate rebate, Product product, CalculateRebateRequest request, out bool isValid)
    {
        isValid = false;

        // Validate rebate exists
        if (rebate == null)
            return 0;

        // Validate product exists
        if (product == null)
            return 0;

        // Validate product supports this incentive type
        if (!product.SupportedIncentives.HasFlag(SupportedIncentiveType.FixedRateRebate))
            return 0;

        // Validate required values
        if (rebate.Percentage <= 0 || product.Price <= 0 || request.Volume <= 0)
            return 0;

        isValid = true;
        return product.Price * rebate.Percentage * request.Volume;
    }
}