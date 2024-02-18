using esplay.persistence;

namespace esplay.processing;

public interface ICommand<T>
    where T : IAggregate<T>
{
    Task<ActionResult<T>> Add(T item);
    Task<ActionResult<T>> Add(params T[] items);
    Task<ActionResult<T>> Update(T item);
    Task<ActionResult<T>> Delete(T item);
}