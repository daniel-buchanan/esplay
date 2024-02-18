using esplay.core;
using esplay.persistence;
using ITimer = esplay.core.ITimer;

namespace esplay.stream;

public class AggregateBuilder : IAggregateBuilder
{
    private readonly ILogger _logger;
    private readonly ITimer _timer;
    private readonly IEventStream _stream;

    public AggregateBuilder(
        ILogger logger,
        ITimer timer,
        IEventStream stream)
    {
        _logger = logger;
        _timer = timer;
        _stream = stream;
    }

    public async Task<T?> Build<T>(Guid id, int fromVersion = 0, T? onExisting = default)
        where T: IAggregate<T>, new()
    {
        var events = await _timer.Time($"IEventStream.ForAggregate<{typeof(T).Name}>", async () => await _stream.ForAggregate(id, fromVersion));
        var item = onExisting;

        foreach (var e in events)
        {
            var result = await _timer.Time($"AggregateBuilder.ApplyEvent", async () => await ApplyEvent(e, item));
            if(!result.Changed)
                await _logger.Log($"Event {e.Id} not applied");
            item = result.Result;
            item.WithVersion(e.Version);
        }
        
        if(item != null)
            item.WithUpdatedAt(DateTimeOffset.UtcNow);

        return item;
    }

    private async Task<EventApplicationResult<T>> ApplyEvent<T>(IEvent e, T? item)
        where T: IAggregate<T>, new()
    {
        var incoming = e.GetDetailsAs<T>();
        incoming.WithVersion(e.Version);
        
        item ??= new T();
        item.WithId(e.AggregateId);
        
        var changed = await _timer.Time($"IAggregate.Mutate<{typeof(T).Name}>", 
            async () => await item.Mutate(incoming));

        if(e.Type == EventType.Create)
            item.WithCreatedAt(DateTimeOffset.UtcNow);
        
        if(changed) return EventApplicationResult.Changed(item);
        return EventApplicationResult.Unchanged(item);
    }
}