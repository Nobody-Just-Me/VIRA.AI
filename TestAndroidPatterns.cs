using System;
using System.Threading.Tasks;
using VIRA.Shared.Models;
using VIRA.Shared.Services;
using VIRA.Shared.Tests;

/// <summary>
/// Standalone test runner for greeting patterns
/// Run with: dotnet script TestAndroidPatterns.cs
/// </summary>
class TestRunner
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== VIRA Greeting Patterns Test Suite ===\n");
        
        try
        {
            var tests = new GreetingPatternsTests();
            
            Console.WriteLine("Running: TestGreetingPatterns");
            await tests.TestGreetingPatterns();
            Console.WriteLine();
            
            Console.WriteLine("Running: TestStatusQueryPatterns");
            await tests.TestStatusQueryPatterns();
            Console.WriteLine();
            
            Console.WriteLine("Running: TestThankYouPatterns");
            await tests.TestThankYouPatterns();
            Console.WriteLine();
            
            Console.WriteLine("Running: TestGoodbyePatterns");
            await tests.TestGoodbyePatterns();
            Console.WriteLine();
            
            Console.WriteLine("Running: TestBilingualSupport");
            tests.TestBilingualSupport();
            Console.WriteLine();
            
            Console.WriteLine("✅ All tests passed!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Test failed: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            Environment.Exit(1);
        }
    }
}
