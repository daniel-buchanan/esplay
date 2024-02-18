using System.Diagnostics;

namespace esplay.core;

public interface ITimer
{
    Task Time(string key, Func<Task> method);
    Task Time(string key, Action method);
    Task<T> Time<T>(string key, Func<Task<T>> method);
    Task<T> Time<T>(string key, Func<T> method);
}

public class Timer : ITimer
{
    private readonly ILogger _logger;

    public Timer(ILogger logger) => _logger = logger;

    public async Task Time(string key, Func<Task> method)
    {
        var stopwatch = new Stopwatch();
        await _logger.Log($"[{key}] Start");
        stopwatch.Start();
        await method();
        stopwatch.Stop();
        await _logger.Log($"[{key}] End. Took {stopwatch.ElapsedMilliseconds}ms");
    }

    public async Task Time(string key, Action method)
    {
        var stopwatch = new Stopwatch();
        await _logger.Log($"[{key}] Start");
        stopwatch.Start();
        method();
        stopwatch.Stop();
        await _logger.Log($"[{key}] End. Took {stopwatch.ElapsedMilliseconds}ms");
    }

    public async Task<T> Time<T>(string key, Func<Task<T>> method)
    {
        var stopwatch = new Stopwatch();
        await _logger.Log($"[{key}] Start");
        stopwatch.Start();
        var result = await method();
        stopwatch.Stop();
        await _logger.Log($"[{key}] End. Took {stopwatch.ElapsedMilliseconds}ms");
        return result;
    }

    public async Task<T> Time<T>(string key, Func<T> method)
    {
        var stopwatch = new Stopwatch();
        await _logger.Log($"[{key}] Start");
        stopwatch.Start();
        var result = method();
        stopwatch.Stop();
        await _logger.Log($"[{key}] End. Took {stopwatch.ElapsedMilliseconds}ms");
        return result;
    }
}