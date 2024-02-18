namespace esplay.persistence;

public interface IRepository<T>
    where T : IAggregate
{
    Task<T?> Single(Predicate<T> query);
    Task<IEnumerable<T>> Fetch(Func<T, bool> query);
    Task<IEnumerable<T>> FetchAll();
    Task Add(T item);
    Task Add(params T[] items);
    Task Update(Guid id, T item);
    Task Remove(Predicate<T> query);
    Task<bool> Exists(Guid itemId);
}