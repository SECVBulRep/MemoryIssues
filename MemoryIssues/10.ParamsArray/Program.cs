using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;


[SimpleJob]
[MemoryDiagnoser]
public class ParamsAllocationBenchmark
{
    [Params(1, 5, 10)]
    public int Count;

    [Benchmark]
    public int WithParamsArray()
    {
        return SumWithParams(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
    }

    [Benchmark]
    public int WithOverload()
    {
        return SumOverload(1, 2, 3, 4, 5, 6, 7, 8, 9, 10);
    }

    private int SumWithParams(params int[] numbers)
    {
        int sum = 0;
        foreach (var number in numbers)
        {
            sum += number;
        }
        return sum;
    }

    private int SumOverload(int n1, int n2, int n3, int n4, int n5, int n6, int n7, int n8, int n9, int n10)
    {
        return n1 + n2 + n3 + n4 + n5 + n6 + n7 + n8 + n9 + n10;
    }
}

class Program
{
    static void Main(string[] args)
    {
        BenchmarkRunner.Run<ParamsAllocationBenchmark>();
    }
}