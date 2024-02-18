namespace esplay.core;

public static class AsyncExtensions
{
    public static async Task<List<T>> ToListAsync<T>(this Task<IEnumerable<T>> self)
    {
        var result = await self;
        return result?.ToList() ?? new List<T>();
    }
    
    public static async Task<List<T>> ToListAsync<T>(this Task<IOrderedEnumerable<T>> self)
    {
        var result = await self;
        return result?.ToList() ?? new List<T>();
    }
}