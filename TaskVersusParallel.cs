using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
//https://www.youtube.com/watch?v=GE-phyI6lEM
//https://mjebrahimi.github.io/DotNet-Collections-Benchmark/Benchmark-SearchTryGetValue-Allocated.html
//https://github.com/dotnet/performance/blob/main/src/benchmarks/micro/README.md#private-runtime-builds
//https://stackoverflow.com/questions/67010378/how-to-populate-and-return-listt-from-parallel-foreach-using-partitioning
//https://habr.com/ru/articles/797777/
BenchmarkSwitcher switcher = new(Assembly.GetExecutingAssembly());
switcher.Run(args);

[Benchmark]
    public async Task TaskWhenAllFix()
    {
        var tasks = _dataSet.Select(_ =>
        Task.Run(() =>
        {
            for (var i = 0; i < CpuWorkIterations; i++)
            {
                Random.Shared.Next();
            }

            return Task.CompletedTask;
        })).ToArray();

        await Task.WhenAll(tasks);
    }

    [Benchmark]
    public async Task TaskWhenAllFixOptPrefer()
    {
        var tasks = _dataSet
            .Select(_ =>
                Task.Factory.StartNew(
                (obj) =>
                {
                    for (var i = 0; i < CpuWorkIterations; i++)
                    {
                        Random.Shared.Next();
                    }
                },
                CancellationToken.None,
                TaskCreationOptions.PreferFairness))
            .ToArray();

        await Task.WhenAll(tasks);
    }

    [Benchmark]
    public async Task TaskWhenAllFixOptPreferLong()
    {
        var tasks = _dataSet
            .Select(_ =>
                Task.Factory.StartNew(
                (obj) =>
                {
                    for (var i = 0; i < CpuWorkIterations; i++)
                    {
                        Random.Shared.Next();
                    }
                },
                CancellationToken.None,
                TaskCreationOptions.PreferFairness | TaskCreationOptions.LongRunning))
            .ToArray();

        await Task.WhenAll(tasks);
    }
