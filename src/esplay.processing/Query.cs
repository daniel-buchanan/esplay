using esplay.persistence;

namespace esplay.processing;

public class Query<T> : IQuery<T>
    where T: IAggregate<T>
{
    private readonly IRepository<T> _repository;

    public Query(IRepository<T> repository) => _repository = repository;

    public Task<T?> ById(Guid id)
        => _repository.Single(x => x.Id == id);

    public Task<T?> Single(Predicate<T> query)
        => _repository.Single(query);

    public Task<IEnumerable<T>> Find(Func<T, bool> query)
        => _repository.Fetch(query);

    public Task<IEnumerable<T>> All()
        => _repository.FetchAll();
}