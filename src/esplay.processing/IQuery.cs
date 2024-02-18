using esplay.persistence;

namespace esplay.processing;

public interface IQuery<T>
    where T: IAggregate<T>
{
    Task<T?> ById(Guid id);
    Task<T?> Single(Predicate<T> query);
    Task<IEnumerable<T>> Find(Func<T, bool> query);
    Task<IEnumerable<T>> All();
}