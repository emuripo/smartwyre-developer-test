using Smartwyre.DeveloperTest.Types;
using System;
using System.Collections.Generic;

namespace Smartwyre.DeveloperTest.Services;

/// <summary>
/// Interface for resolving incentive calculators based on incentive type.
/// </summary>
public interface IIncentiveCalculatorResolver
{
    /// <summary>
    /// Gets the appropriate calculator for the specified incentive type.
    /// </summary>
    IIncentiveCalculator GetCalculator(IncentiveType incentiveType);
}

/// <summary>
/// Factory implementation for resolving incentive calculators.
/// Makes it easy to add new incentive types without modifying existing code.
/// </summary>
public class IncentiveCalculatorResolver : IIncentiveCalculatorResolver
{
    private readonly Dictionary<IncentiveType, IIncentiveCalculator> _calculators;

    public IncentiveCalculatorResolver()
    {
        _calculators = new Dictionary<IncentiveType, IIncentiveCalculator>
        {
            { IncentiveType.FixedCashAmount, new FixedCashAmountCalculator() },
            { IncentiveType.FixedRateRebate, new FixedRateRebateCalculator() },
            { IncentiveType.AmountPerUom, new AmountPerUomCalculator() }
        };
    }

    public IIncentiveCalculator GetCalculator(IncentiveType incentiveType)
    {
        if (!_calculators.TryGetValue(incentiveType, out var calculator))
        {
            throw new NotSupportedException($"Incentive type {incentiveType} is not supported.");
        }
        
        return calculator;
    }
}