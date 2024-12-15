// See https://aka.ms/new-console-template for more information

using System;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

[SimpleJob]
[MemoryDiagnoser]
public class TupleComparison
{
    private const int OperationsCount = 10000;

    [Benchmark]
    public (int, string) CreateAndAccessValueTuple()
    {
        (int, string) valueTuple = (42, "Hello");
        int number = valueTuple.Item1;
        string text = valueTuple.Item2;
        return (number, text);
    }

    [Benchmark]
    public Tuple<int, string> CreateAndAccessTuple()
    {
        Tuple<int, string> tuple = Tuple.Create(42, "Hello");
        int number = tuple.Item1;
        string text = tuple.Item2;
        return tuple;
    }

    [Benchmark]
    public int SumWithValueTuple()
    {
        var result = 0;
        for (int i = 0; i < OperationsCount; i++)
        {
            (int, int) valueTuple = (i, i * 2);
            result += valueTuple.Item1 + valueTuple.Item2;
        }
        return result;
    }

    [Benchmark]
    public int SumWithTuple()
    {
        var result = 0;
        for (int i = 0; i < OperationsCount; i++)
        {
            Tuple<int, int> tuple = Tuple.Create(i, i * 2);
            result += tuple.Item1 + tuple.Item2;
        }
        return result;
    }

    [Benchmark]
    public void PassValueTupleToMethod()
    {
        for (int i = 0; i < OperationsCount; i++)
        {
            ProcessValueTuple((i, i * 2));
        }
    }

    [Benchmark]
    public void PassTupleToMethod()
    {
        for (int i = 0; i < OperationsCount; i++)
        {
            ProcessTuple(Tuple.Create(i, i * 2));
        }
    }

    private void ProcessValueTuple((int, int) valueTuple)
    {
        int sum = valueTuple.Item1 + valueTuple.Item2;
    }

    private void ProcessTuple(Tuple<int, int> tuple)
    {
        int sum = tuple.Item1 + tuple.Item2;
    }
    
    
    private readonly (int, int, int, int, int, int, int) largeValueTuple = (1, 2, 3, 4, 5, 6, 7);
    private readonly Tuple<int, int, int, int, int, int, int> largeTuple = 
        Tuple.Create(1, 2, 3, 4, 5, 6, 7);

    [Benchmark]
    public int AccessValueTuple()
    {
        var tuple = largeValueTuple;
        return tuple.Item1 + tuple.Item2 + tuple.Item3 + tuple.Item4 +
               tuple.Item5 + tuple.Item6 + tuple.Item7 ;
    }

    [Benchmark]
    public int AccessTuple()
    {
        var tuple = largeTuple;
        return tuple.Item1 + tuple.Item2 + tuple.Item3 + tuple.Item4 +
               tuple.Item5 + tuple.Item6 + tuple.Item7 + tuple.Item7;
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<TupleComparison>();
    }
}
