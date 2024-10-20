using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.Threading.Tasks;


/// <summary>
///  Когернетностьь кешей. Страница 97
/// </summary>
[SimpleJob(baseline: true)]
[MemoryDiagnoser]
public class Benchy
{
   
    private const int Size = 100_000_000;

    [Benchmark(Baseline = true)]
    public void Test1()
    {
        const int offset = 1;
        const int gap = 0;
        var sharedData = new int[4 * offset + gap * offset];

       
        DoFalseSharingTest(4, sharedData, offset, gap);
    }

    [Benchmark]
    public void Test2()
    {
        const int offset = 16;
        const int gap = 0;
        var sharedData = new int[4 * offset + gap * offset];

        DoFalseSharingTest(4, sharedData, offset, gap);
    }

    [Benchmark]
    public void Test3()
    {
        const int offset = 16;
        const int gap = 16;
        var sharedData = new int[4 * offset + gap * offset];

        DoFalseSharingTest(4, sharedData, offset, gap);
    }

    
    private static void DoFalseSharingTest(int threadsCount, int[] sharedData, int offset, int gap)
    {
        Parallel.For(0, threadsCount, i =>
        {
            var index = i + gap;
            for (var j = 0; j < Size; ++j)
            {
                sharedData[index * offset] += 1;
            }
        });
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<Benchy>();
    }
}