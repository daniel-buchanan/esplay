namespace esplay.processing;

public static class ActionResult
{
    public static ActionResult<T> Success<T>()
        => new ActionResult<T>(true);
    
    public static ActionResult<T> Success<T>(T result)
        => new ActionResult<T>(true, result);

    public static ActionResult<T> Failed<T>(params string[] errors)
        => new ActionResult<T>(false, default, errors);
    public static ActionResult<T> Failed<T>(T result, params string[] errors)
        => new ActionResult<T>(false, result, errors);
}

public sealed class ActionResult<T>
{
    public ActionResult(bool successful, T? result = default, IEnumerable<string>? errors = null)
    {
        Successful = successful;
        Result = result;
        Errors = errors;
    }
    
    public bool Successful { get; }
    public T? Result { get; }
    public IEnumerable<string>? Errors { get; }
}