namespace esplay.stream;

public interface IEventStream
{
    Task<IOrderedEnumerable<IEvent>> ForAggregate(Guid id, int fromVersion = 0);
    Task<IEnumerable<IEvent>> Where(Func<IEvent, bool> query);
    Task Push<T>(Guid aggregateId, EventType type, T details);
}