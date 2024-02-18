namespace esplay.stream;

public interface IEvent
{
    int Id { get; }
    Guid AggregateId { get; }
    EventType Type { get; }
    int Version { get; }
    string Details { get; }
    
    string Fingerprint { get; }
    DateTimeOffset CreatedAt { get; }

    IEvent WithId(int id);
    IEvent WithVersion(int version);
    IEvent For(EventType type);
    IEvent ForAggregate(Guid id);
    IEvent WithDetails<T>(T details);

    IEvent WithFingerprint(string fingerprint);
    T GetDetailsAs<T>();
}