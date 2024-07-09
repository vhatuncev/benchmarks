using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

BenchmarkSwitcher switcher = new(Assembly.GetExecutingAssembly());
switcher.Run(args);

[ShortRunJob]
public class BenchmarkSimulatedIo
{
    [Params(1, 10, 100)]
    public int CollectionCount;

    [Params(1, 10, 100, 1000)]
    public int SimulatedIoDelays;

    [Benchmark]
    public async Task TaskWhenAll()
    {
        var tasks = Enumerable
            .Range(0, CollectionCount)
            .Select(async _ => await Task.Delay(SimulatedIoDelays))
            .ToArray();

        await Task.WhenAll(tasks);
    }

    [Benchmark]
    public async Task ParallelForEach() =>
        await Parallel.ForEachAsync(
            Enumerable.Range(0, CollectionCount),
            cancellationToken: default,
            async (i, ct) => await Task.Delay(SimulatedIoDelays, ct));
}

[ShortRunJob]
public class BenchmarkSimulatedCpu
{
    private int[]? _dataSet;
    
    [Params(1, 10, 100)]
    public int CollectionCount;

    [Params(1000, 10_000, 100_000, 1_000_000)]
    public int CpuWorkIterations;

    [GlobalSetup]
    public void GlobalSetup() =>
        _dataSet = Enumerable.Range(0, CollectionCount).ToArray();

    [Benchmark]
    public async Task TaskWhenAll()
    {
        var tasks = _dataSet!.Select(_ =>
        {
            for (var i = 0; i < CpuWorkIterations; i++)
            {
                Random.Shared.Next();
            }
            return Task.CompletedTask;
        }).ToArray();

        await Task.WhenAll(tasks);
    }

    [Benchmark]
    public async Task ParallelForEach() =>
        await Parallel.ForEachAsync(
            _dataSet!,
            cancellationToken: default,
            (_, ct) =>
            {
                for (var i = 0; i < CpuWorkIterations; i++)
                {
                    Random.Shared.Next();
                }
                return ValueTask.CompletedTask;
            });
