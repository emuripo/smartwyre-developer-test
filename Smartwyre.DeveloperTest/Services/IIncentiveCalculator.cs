using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Services;

/// <summary>
/// Interface for incentive calculation strategies.
/// Each IncentiveType should have its own implementation containing validation and calculation logic.
/// </summary>
public interface IIncentiveCalculator
{
    /// <summary>
    /// Calculates the rebate amount for a specific incentive type.
    /// Returns the calculated amount if valid, or 0 if invalid.
    /// </summary>
    decimal Calculate(Rebate rebate, Product product, CalculateRebateRequest request, out bool isValid);
    
    /// <summary>
    /// The incentive type this calculator handles.
    /// </summary>
    IncentiveType IncentiveType { get; }
}