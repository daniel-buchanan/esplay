using esplay.persistence;

namespace esplay.stream;

public interface IAggregateBuilder
{
    Task<T?> Build<T>(Guid id, int fromVersion = 0, T? onExisting = default)
        where T: IAggregate<T>, new();
}