namespace esplay.stream;

public static class EventApplicationResult
{
    public static EventApplicationResult<T> Changed<T>(T result)
        => new EventApplicationResult<T>(true, result);

    public static EventApplicationResult<T> Unchanged<T>(T result)
        => new EventApplicationResult<T>(false, result);
}

public sealed class EventApplicationResult<T>(bool changed, T result)
{
    public bool Changed { get; } = changed;
    public T Result { get; } = result;
}