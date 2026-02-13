using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services;

public class FixedCashAmountCalculator : IIncentiveCalculator
{
    public IncentiveType IncentiveType => IncentiveType.FixedCashAmount;

    public decimal Calculate(Rebate rebate, Product product, CalculateRebateRequest request, out bool isValid)
    {
        isValid = false;

        // Validate rebate exists
        if (rebate == null)
            return 0;

        // Validate product supports this incentive type
        if (!product.SupportedIncentives.HasFlag(SupportedIncentiveType.FixedCashAmount))
            return 0;

        // Validate rebate amount
        if (rebate.Amount <= 0)
            return 0;

        isValid = true;
        return rebate.Amount;
    }
}