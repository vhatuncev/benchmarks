using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
//https://www.youtube.com/watch?v=GE-phyI6lEM
//https://mjebrahimi.github.io/DotNet-Collections-Benchmark/Benchmark-SearchTryGetValue-Allocated.html
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
