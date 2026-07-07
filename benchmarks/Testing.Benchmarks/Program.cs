using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Atya.Governance.Testing.Benchmarks;

/// <summary>
/// Runs the governance testing benchmarks.
/// </summary>
public static class Program
{
    /// <summary>
    /// Starts the benchmark runner.
    /// </summary>
    /// <param name="args">Command-line arguments.</param>
    public static void Main(string[] args)
    {
        _ = args;
        BenchmarkRunner.Run<TemplateBenchmarks>();
    }
}

/// <summary>
/// Benchmarks simple governance testing helper access.
/// </summary>
[MemoryDiagnoser]
public class TemplateBenchmarks
{
    private readonly string _value = "Atya.Governance.Testing";

    /// <summary>
    /// Reads the length of a representative value.
    /// </summary>
    /// <returns>The value length.</returns>
    [Benchmark]
    public int ReadStarterValueLength()
    {
        return _value.Length;
    }
}
