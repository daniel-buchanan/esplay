using System.Linq.Expressions;

namespace esplay.persistence;

public interface IAggregate
{
    Guid Id { get; }
    int Version { get; }
    DateTimeOffset CreatedAt { get; }
    DateTimeOffset UpdatedAt { get; }

    IAggregate WithId(Guid id);
    IAggregate WithVersion(int version);
    IAggregate WithCreatedAt(DateTimeOffset date);
    IAggregate WithUpdatedAt(DateTimeOffset date);
}

public interface IAggregate<T> : IAggregate
    where T: IAggregate
{
    IEnumerable<Expression<Func<T, object>>> KeyProperties { get; }
    Task<bool> Mutate(T incoming);
}