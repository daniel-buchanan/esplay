using Newtonsoft.Json;

namespace esplay.stream;

public class Event : IEvent
{
    public int Id { get; private set; }
    
    public Guid AggregateId { get; private set; }
    
    public EventType Type { get; private set; }
    
    public int Version { get; private set; }
    
    public string Details { get; private set; }
    
    public string Fingerprint { get; private set; }
    
    public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;

    public static IEvent Create() => new Event();

    public IEvent WithId(int id)
    {
        Id = id;
        return this;
    }
    
    public IEvent WithVersion(int version)
    {
        Version = version;
        return this;
    }

    public IEvent For(EventType type)
    {
        Type = type;
        return this;
    }

    public IEvent ForAggregate(Guid id)
    {
        AggregateId = id;
        return this;
    }

    public IEvent WithDetails<T>(T details)
    {
        Details = JsonConvert.SerializeObject(details);
        return this;
    }

    public IEvent WithFingerprint(string fingerprint)
    {
        Fingerprint = fingerprint;
        return this;
    }

    public T GetDetailsAs<T>()
        => JsonConvert.DeserializeObject<T>(Details);
}