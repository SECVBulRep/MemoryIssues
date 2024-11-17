// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.NoEmit;

//лОкальнОсть типОв данных 266
public struct SmallStruct
{
    public int Value1;
    public int Value2;
}
public class SmallClass
{
    public int Value1;
    public int Value2;
}

[SimpleJob]
[MemoryDiagnoser]
public class Benchy
{
    private SmallClass[] classes ;
    private SmallStruct[] structs;

   

    [GlobalSetup]
    public void Setup()
    {
        classes = new SmallClass[1_000_000];

        for (int i = 0; i < classes.Length; i++)
        {
            classes[i] = new SmallClass();
            classes[i].Value1 = 0;
            classes[i].Value2 = 0;
        }

        structs= new SmallStruct[1_000_000];
    }
    
    
    [Benchmark]
    public int ClassArrayAccess()
    {
        int result = 0;
        for (int i = 0; i < classes.Length; i++)
            result += Helper2(classes, i);
        return result;
    }
    
    
    [Benchmark]
    public int StructArrayAccess()
    {
        int result = 0;
        for (int i = 0; i < structs.Length; i++)
            result += Helper1(structs, i);
        return result;
    }
    
    public int Helper1(SmallStruct [] data, int index)
    {
        return data[index].Value1;
    }
    public int Helper2(SmallClass [] data, int index)
    {
        return data[index].Value1;
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<Benchy>();
    }
}