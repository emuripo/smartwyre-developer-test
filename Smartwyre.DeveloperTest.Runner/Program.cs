#nullable enable
using System;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Types;

namespace Smartwyre.DeveloperTest.Runner;

/// <summary>
/// Console application runner for the refactored RebateService.
/// Usage: dotnet run <RebateIdentifier> <ProductIdentifier> <Volume>
/// Example: dotnet run "REBATE001" "PROD001" 100.5
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Smartwyre Rebate Calculator");
        Console.WriteLine("===============================");

        try
        {
            CalculateRebateRequest? request;
            
            if (args.Length == 3)
            {
                request = BuildRequestFromArgs(args);
                if (request == null)
                {
                    PrintUsage();
                    return;
                }
            }
            else
            {
                request = BuildRequestInteractively();
                if (request == null)
                {
                    return;
                }
            }

            Console.WriteLine($"Processing rebate calculation:");
            Console.WriteLine($"   Rebate ID: {request.RebateIdentifier}");
            Console.WriteLine($"   Product ID: {request.ProductIdentifier}");
            Console.WriteLine($"   Volume: {request.Volume}");
            Console.WriteLine();

            // Execute the calculation using refactored service
            var rebateService = new RebateService();
            var result = rebateService.Calculate(request);

            PrintResult(result);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            Console.WriteLine();
            PrintUsage();
        }
    }

    private static CalculateRebateRequest? BuildRequestFromArgs(string[] args)
    {
        string rebateIdentifier = args[0];
        string productIdentifier = args[1];
        
        if (!decimal.TryParse(args[2], out decimal volume))
        {
            Console.WriteLine("Error: Volume must be a valid decimal number.");
            return null;
        }

        return new CalculateRebateRequest
        {
            RebateIdentifier = rebateIdentifier,
            ProductIdentifier = productIdentifier,
            Volume = volume
        };
    }

    private static CalculateRebateRequest? BuildRequestInteractively()
    {
        Console.WriteLine("Starting interactive mode...");
        Console.WriteLine();

        Console.Write("Enter Rebate Identifier: ");
        string? rebateId = Console.ReadLine();

        Console.Write("Enter Product Identifier: ");
        string? productId = Console.ReadLine();

        Console.Write("Enter Volume: ");
        string? volumeInput = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(rebateId))
        {
            Console.WriteLine("Error: Rebate Identifier cannot be empty.");
            return null;
        }

        if (string.IsNullOrWhiteSpace(productId))
        {
            Console.WriteLine("Error: Product Identifier cannot be empty.");
            return null;
        }

        if (!decimal.TryParse(volumeInput, out decimal volume))
        {
            Console.WriteLine("Error: Volume must be a valid decimal number.");
            return null;
        }

        Console.WriteLine();
        return new CalculateRebateRequest
        {
            RebateIdentifier = rebateId,
            ProductIdentifier = productId,
            Volume = volume
        };
    }

    private static void PrintResult(CalculateRebateResult result)
    {
        if (result.Success)
        {
            Console.WriteLine("Rebate calculation successful!");
            Console.WriteLine($"   Message: {result.Message}");
            Console.WriteLine($"   Amount: {result.Amount:C}");
        }
        else
        {
            Console.WriteLine("Rebate calculation failed.");
            Console.WriteLine($"   Message: {result.Message}");
            Console.WriteLine($"   Amount: {result.Amount:C}");
        }
    }

    private static void PrintUsage()
    {
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("  dotnet run -- <RebateIdentifier> <ProductIdentifier> <Volume>");
        Console.WriteLine("  dotnet run --project <ProjectPath> -- <RebateIdentifier> <ProductIdentifier> <Volume>");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  dotnet run -- \"REBATE001\" \"PROD001\" 100");
        Console.WriteLine("  dotnet run -- \"CASH50\" \"WIDGET123\" 25.75");
        Console.WriteLine("  dotnet run --project Smartwyre.DeveloperTest.Runner -- \"REBATE001\" \"PROD001\" 100");
        Console.WriteLine();
        Console.WriteLine("Or run without arguments for interactive mode.");
    }
}
