namespace esplay.persistence;

public class Repository<T> : IRepository<T>
    where T: IAggregate
{
    private readonly List<T> _source = [];
    private readonly esplay.core.ITimer _timer;

    public Repository(esplay.core.ITimer timer) => _timer = timer;
    
    public async Task<T?> Single(Predicate<T> query) 
        => await _timer.Time($"IRepository<{typeof(T).Name}>.Single", () => _source.Find(query));

    public async Task<IEnumerable<T>> Fetch(Func<T, bool> query)
        => await _timer.Time($"IRepository<{typeof(T).Name}>.Fetch",() => _source.Where(query));

    public async Task<IEnumerable<T>> FetchAll()
        => await _timer.Time($"IRepository<{typeof(T).Name}>.FetchAll", () => _source.AsEnumerable());

    public async Task Add(T item) 
        => await _timer.Time($"IRepository<{typeof(T).Name}>.Add",() => _source.Add(item));

    public async Task Add(params T[] items) 
        => await _timer.Time($"IRepository<{typeof(T).Name}>.Add(params)", () => _source.AddRange(items));

    public async Task Update(Guid id, T item)
    {
        await _timer.Time($"IRepository<{typeof(T).Name}>.Update", () =>
        {
            _source.RemoveAll(i => i.Id == id);
            _source.Add(item);
        });
    }

    public async Task Remove(Predicate<T> query) 
        => await _timer.Time($"IRepository<{typeof(T).Name}>.Add", () => _source.RemoveAll(query));

    public async Task<bool> Exists(Guid itemId) 
        => await _timer.Time($"IRepository<{typeof(T).Name}>.Exists", () => _source.Exists(i => i.Id == itemId));
}