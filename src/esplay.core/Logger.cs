namespace esplay.core;

public interface ILogger
{
    Task Log(string message);
    Task Log(string message, Exception ex);
}

public class Logger : ILogger
{
    public Task Log(string message)
    {
        Console.WriteLine(FormatMessage(message));
        return Task.CompletedTask;
    }

    public Task Log(string message, Exception ex)
    {
        Console.WriteLine(FormatMessage(message));
        Console.WriteLine(FormatMessage(ex.Message));
        if(!string.IsNullOrWhiteSpace(ex.StackTrace))
            Console.WriteLine(FormatMessage(ex.StackTrace));
        return Task.CompletedTask;
    }

    private string FormatMessage(string message)
        => $"[{DateTimeOffset.UtcNow:yyyy-MM-ddThh:mm:ss}] {message}";
}