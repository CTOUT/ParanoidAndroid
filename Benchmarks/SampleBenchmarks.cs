using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace ParanoidAndroid.Benchmarks;

public class SampleBenchmarks
{
    private static int[] data = new int[1024];

    [GlobalSetup]
    public void Setup()
    {
        for (int i = 0; i < data.Length; i++) data[i] = i;
    }

    [Benchmark]
    public int ForSum()
    {
        int sum = 0;
        var local = data;
        for (int i = 0; i < local.Length; i++) sum += local[i];
        return sum;
    }

    [Benchmark]
    public int ForeachSum()
    {
        int sum = 0;
        foreach (var v in data) sum += v;
        return sum;
    }
}

public static class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<SampleBenchmarks>();
    }
}
