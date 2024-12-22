// See https://aka.ms/new-console-template for more information

using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;



// AsyncTaskMethodBuilder<string> asyncTaskMethodBuilder = new AsyncTaskMethodBuilder<string>();
//
// asyncTaskMethodBuilder.SetResult();

// async Task<string> ReadFileAsync(string filename)
// {
//     if (!File.Exists(filename))
//         return string.Empty;
//     return await File.ReadAllTextAsync(filename);
// }


// AsyncValueTaskMethodBuilder<string> asyncValueTaskMethodBuilder = new AsyncValueTaskMethodBuilder<string>();
// asyncValueTaskMethodBuilder.SetResult();
//
// async ValueTask<string> ReadFileAsync2(string filename)
// {
//     if (!File.Exists(filename))
//         return string.Empty;
//     return await File.ReadAllTextAsync(filename);
// }



[SimpleJob]
[MemoryDiagnoser]
public class TaskVsValueTaskBenchmark
{
   
    public Task<int> GetTaskAsync()
    {
        return Task.FromResult(42); 
    }

   
    public ValueTask<int> GetValueTaskAsync()
    {
        return new ValueTask<int>(42); 
    }

    
    [Benchmark]
    public async Task<int> BenchmarkTask()
    {
        return await GetTaskAsync();
    }

    
    [Benchmark]
    public async ValueTask<int> BenchmarkValueTask()
    {
        return await GetValueTaskAsync();
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<TaskVsValueTaskBenchmark>();
    }
}