using VIRA.Shared.Tests;

/// <summary>
/// Simple test runner for RuleBasedProcessor tests
/// </summary>
class TestRuleBasedProcessor
{
    static async Task Main(string[] args)
    {
        try
        {
            var tests = new RuleBasedProcessorTests();
            await tests.RunAllTests();
            Environment.Exit(0);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Test execution failed: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            Environment.Exit(1);
        }
    }
}
