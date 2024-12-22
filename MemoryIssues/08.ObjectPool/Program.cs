// See https://aka.ms/new-console-template for more information

using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

public class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<ObjectPoolVsNewBenchmark>();
        // var pool = new ObjectPool<StringBuilder>(() => new StringBuilder(), 10);
        //
        //
        // var sb = pool.Rent();
        // sb.Append("Using ObjectPool");
        // sb.Clear();
        // pool.Return(sb);
        //
        // sb = pool.Rent();
        // sb.Append("Using ObjectPool 2");
        // sb.Clear();
        // pool.Return(sb);
        
        
    }
}


[SimpleJob]
[MemoryDiagnoser]
public class ObjectPoolVsNewBenchmark
{
    private ObjectPool<StringBuilder> pool;

    [Params(10, 100, 1000)] 
    public int Iterations;

    [GlobalSetup]
    public void Setup()
    {
        pool = new ObjectPool<StringBuilder>(() => new StringBuilder(), 10);
    }

    [Benchmark]
    public void WithObjectPool()
    {
        for (int i = 0; i < Iterations; i++)
        {
            var sb = pool.Rent();
            sb.Append("Using ObjectPool");
            sb.Clear();
            pool.Return(sb);
        }
    }

    [Benchmark]
    public void WithoutObjectPool()
    {
        for (int i = 0; i < Iterations; i++)
        {
            var sb = new StringBuilder();
            sb.Append("Without ObjectPool");
            sb.Clear();
        }
    }
}

public class ObjectPool<T> where T : class
{
    private readonly Func<T> generator;
    private readonly T[] items;
    private T firstItem;

    public ObjectPool(Func<T> generator, int size)
    {
        this.generator = generator ?? throw new ArgumentNullException("generator");
        items = new T[size-1];
    }

    public T Rent()
    {
        // PERF: сначала проверяем первый элемент. Если не получается, RentSlow
        // рассмотрит остальные элементы.
        // Заметим, что первое чтение не синхронизируется. Это
        // сделано сознательно.
        // Блокировка производится, только когда есть кандидат. В худшем
        // случае мы пропустим какие–то недавно возвращенные объекты. Ничего
        // страшного.
        var inst = firstItem;
        if (inst == null || inst != Interlocked.CompareExchange
                (ref firstItem, null, inst))
            inst = RentSlow();
        return inst;
    }

    public void Return(T item)
    {
        if (firstItem == null)
            // Здесь блокировка опущена сознательно.
            // В худшем случае два объекта будут сохранены в одном месте.
            // Это крайне маловероятно, но если случится, то один из объектов
            // будет убран в мусор.
            firstItem = item;
        else
            ReturnSlow(item);
    }

    private T RentSlow()
    {
        for (var i = 0; i < items.Length; i++)
        {
            // Первое чтение не синхронизируется. Это
            // сделано сознательно.
            // Блокировка производится, только когда есть кандидат. В худшем
            // случае мы пропустим какие–то недавно возвращенные объекты. Ничего
            // страшного.
            var inst = items[i];
            if (inst != null)
                if (inst == Interlocked.CompareExchange(ref items[i],
                        null, inst))
                    return inst;
        }

        return generator();
    }

    private void ReturnSlow(T obj)
    {
        for (var i = 0; i < items.Length; i++)
            if (items[i] == null)
            {
                // Здесь блокировка опущена сознательно.
                // В худшем случае два объекта будут сохранены в одном месте.
                // Это крайне маловероятно, но если случится, то один из объектов
                // будет убран в мусор.
                items[i] = obj;
                break;
            }
    }
}