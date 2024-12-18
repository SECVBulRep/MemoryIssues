using System;
using System.IO;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Microsoft.IO; // Подключаем Microsoft.IO для RecyclableMemoryStream


[SimpleJob]
[MemoryDiagnoser]
public class MemoryStreamBenchmark
{
    private const int BufferSize = 1024 * 1024; // 1 MB
    private byte[] buffer;
    private RecyclableMemoryStreamManager recyclableMemoryStreamManager;

    [GlobalSetup]
    public void Setup()
    {
        buffer = new byte[BufferSize];
        new Random().NextBytes(buffer); // Заполняем массив случайными данными
        recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
    }

    [Benchmark]
    public void WriteToMemoryStream()
    {
        using var memoryStream = new MemoryStream();
        memoryStream.Write(buffer, 0, buffer.Length);
        memoryStream.Position = 0;
        memoryStream.Read(buffer, 0, buffer.Length);
    }

    [Benchmark]
    public void WriteToRecyclableMemoryStream()
    {
        using var recyclableMemoryStream = recyclableMemoryStreamManager.GetStream();
        recyclableMemoryStream.Write(buffer, 0, buffer.Length);
        recyclableMemoryStream.Position = 0;
        recyclableMemoryStream.Read(buffer, 0, buffer.Length);
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<MemoryStreamBenchmark>();
    }
}