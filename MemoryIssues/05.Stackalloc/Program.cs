using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

[SimpleJob]
[MemoryDiagnoser]
public class Benchy
{
    private List<BigData> testData;

    [Params(100, 1000, 10000)] 
    public int ListSize { get; set; }

    [GlobalSetup]
    public void Setup()
    {
       
        testData = new List<BigData>(ListSize);
        for (int i = 0; i < ListSize; i++)
        {
            testData.Add(new BigData
            {
                Age = i % 100,
                Description = $"Description {i}"
            });
        }
    }

    [Benchmark]
    public double ProcessEnumerable()
    {
       
        var avg = ProcessData1(testData.Select(x => new DataClass
        {
            Age = x.Age,
            Sex = Helper(x.Description) ? Sex.Female : Sex.Male
        }));
        return avg;
    }

    [Benchmark]
    public double ProcessStackallocWithBatches()
    {
        const int batchSize = 1024;
        double result = 0;

        for (int offset = 0; offset < testData.Count; offset += batchSize)
        {
            int count = Math.Min(batchSize, testData.Count - offset);
            Span<DataStruct> data = stackalloc DataStruct[count];
        
            for (int i = 0; i < count; ++i)
            {
                data[i].Age = testData[offset + i].Age;
                data[i].Sex = Helper(testData[offset + i].Description) ? Sex.Female : Sex.Male;
            }

            result += ProcessData2(data);
        }

        return result;
    }

    private double ProcessData2(ReadOnlySpan<DataStruct> readOnlySpan)
    {
        // Имитируем обработку данных
        double sum = 0;
        foreach (var item in readOnlySpan)
        {
            sum += item.Age;
        }
        return sum / readOnlySpan.Length;
    }

    private bool Helper(string description)
    {
        // Простая проверка
        return description.Length % 2 == 0;
    }

    public double ProcessData1(IEnumerable<DataClass> list)
    {
        // Имитируем обработку данных
        return list.Average(x => x.Age);
    }

    public class BigData
    {
        public double Age;
        public string Description;
    }

    public class DataClass
    {
        public double Age { get; set; }
        public Sex Sex { get; set; }
    }

    public struct DataStruct
    {
        public double Age { get; set; }
        public Sex Sex { get; set; }
    }

    public enum Sex
    {
        Male = 0,
        Female = 1
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<Benchy>();
    }
}
