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
        Console.WriteLine("🏢 Smartwyre Rebate Calculator");
        Console.WriteLine("===============================");

        try
        {
            if (args.Length == 3)
            {
                // Parse command line arguments
                string rebateIdentifier = args[0];
                string productIdentifier = args[1];
                
                if (!decimal.TryParse(args[2], out decimal volume))
                {
                    Console.WriteLine("❌ Error: Volume must be a valid decimal number.");
                    PrintUsage();
                    return;
                }

                // Create request from CLI arguments
                var request = new CalculateRebateRequest
                {
                    RebateIdentifier = rebateIdentifier,
                    ProductIdentifier = productIdentifier,
                    Volume = volume
                };

                Console.WriteLine($"📋 Processing rebate calculation:");
                Console.WriteLine($"   Rebate ID: {request.RebateIdentifier}");
                Console.WriteLine($"   Product ID: {request.ProductIdentifier}");
                Console.WriteLine($"   Volume: {request.Volume}");
                Console.WriteLine();

                // Execute the calculation using refactored service
                var rebateService = new RebateService();
                var result = rebateService.Calculate(request);

                // Display results
                if (result.Success)
                {
                    Console.WriteLine("✅ Rebate calculation successful!");
                    Console.WriteLine("   Rebate has been calculated and stored.");
                }
                else
                {
                    Console.WriteLine("❌ Rebate calculation failed.");
                    Console.WriteLine("   Please check that the rebate and product are valid and compatible.");
                }
            }
            else
            {
                // Interactive mode when no CLI arguments provided
                Console.WriteLine("🎯 Starting interactive mode...");
                Console.WriteLine();

                Console.Write("Enter Rebate Identifier: ");
                string rebateId = Console.ReadLine();

                Console.Write("Enter Product Identifier: ");
                string productId = Console.ReadLine();

                Console.Write("Enter Volume: ");
                string volumeInput = Console.ReadLine();

                if (!decimal.TryParse(volumeInput, out decimal volume))
                {
                    Console.WriteLine("❌ Error: Volume must be a valid decimal number.");
                    return;
                }

                var request = new CalculateRebateRequest
                {
                    RebateIdentifier = rebateId,
                    ProductIdentifier = productId,
                    Volume = volume
                };

                Console.WriteLine();
                Console.WriteLine($"📋 Processing rebate calculation:");
                Console.WriteLine($"   Rebate ID: {request.RebateIdentifier}");
                Console.WriteLine($"   Product ID: {request.ProductIdentifier}");
                Console.WriteLine($"   Volume: {request.Volume}");
                Console.WriteLine();

                var rebateService = new RebateService();
                var result = rebateService.Calculate(request);

                if (result.Success)
                {
                    Console.WriteLine("✅ Rebate calculation successful!");
                    Console.WriteLine("   Rebate has been calculated and stored.");
                }
                else
                {
                    Console.WriteLine("❌ Rebate calculation failed.");
                    Console.WriteLine("   Please check that the rebate and product are valid and compatible.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"💥 An error occurred: {ex.Message}");
            Console.WriteLine();
            PrintUsage();
        }

        Console.WriteLine();
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    private static void PrintUsage()
    {
        Console.WriteLine();
        Console.WriteLine("📖 Usage:");
        Console.WriteLine("  dotnet run <RebateIdentifier> <ProductIdentifier> <Volume>");
        Console.WriteLine();
        Console.WriteLine("📝 Examples:");
        Console.WriteLine("  dotnet run \"REBATE001\" \"PROD001\" 100");
        Console.WriteLine("  dotnet run \"CASH50\" \"WIDGET123\" 25.75");
        Console.WriteLine();
        Console.WriteLine("Or run without arguments for interactive mode.");
    }
}
