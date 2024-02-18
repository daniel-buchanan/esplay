using esplay.persistence;
using esplay.stream;

namespace esplay.processing;

public class Command<T> : ICommand<T> 
    where T : IAggregate<T>, new()
{
    private readonly IRepository<T> _repository;
    private readonly IEventStream _eventStream;
    private readonly IAggregateBuilder _aggregateBuilder;

    public Command(
        IRepository<T> repository, 
        IEventStream eventStream, 
        IAggregateBuilder aggregateBuilder)
    {
        _repository = repository;
        _eventStream = eventStream;
        _aggregateBuilder = aggregateBuilder;
    }

    public async Task<ActionResult<T>> Add(T item)
    {
        var itemId = Guid.NewGuid();
        item.WithId(itemId);

        var existing = await _repository.Exists(itemId);
        if (existing)
        {
            return ActionResult.Failed(item, $"Item {itemId} already exists, cannot CREATE");
        }
        
        await _eventStream.Push(itemId, EventType.Create, item);
        var x = await _aggregateBuilder.Build<T>(itemId, item.Version);
        
        if (x == null) 
            return ActionResult.Failed(item, $"Failed to build {itemId} for {typeof(T).Name} for CREATE");
        
        await _repository.Add(x);
        return ActionResult.Success(x);
    }

    public async Task<ActionResult<T>> Add(params T[] items)
    {
        var results = new List<ActionResult<T>>();
        foreach (var i in items) 
            results.Add(await Add(i));

        var success = results.TrueForAll(r => r.Successful);
        var errors = results.SelectMany(r => r.Errors ?? Array.Empty<string>())
            .ToArray();
        return !success ? 
            ActionResult.Failed<T>(errors) : 
            ActionResult.Success<T>();
    }

    public async Task<ActionResult<T>> Update(T item)
    {
        var existing = await _repository.Single(x => x.Id == item.Id);
        
        await _eventStream.Push(item.Id, EventType.Update, item);
        var x = await _aggregateBuilder.Build(item.Id, item.Version, existing);

        if (x == null) 
            return ActionResult.Failed(item, $"Failed to build {item.Id} for {typeof(T).Name} for UPDATE");
        
        await _repository.Update(item.Id, x);
        return ActionResult.Success(x);
    }

    public async Task<ActionResult<T>> Delete(T item)
    {
        var existing = await _repository.Exists(item.Id);
        if (!existing)
            return ActionResult.Failed<T>($"Item {item.Id} does not exist, cannot DELETE");
        
        await _eventStream.Push(item.Id, EventType.Delete, item);
        await _repository.Remove(i => i.Id == item.Id);
        
        return ActionResult.Success(item);
    }
}