using esplay.core;
using ITimer = esplay.core.ITimer;

namespace esplay.stream;

public class EventStream : IEventStream
{
    private int _idSerial = 0;
    private readonly List<IEvent> _stream = [];
    private readonly ITimer _timer;
    private readonly IFingerprintService _fingerprintService;

    public EventStream(ITimer timer, IFingerprintService fingerprintService)
    {
        _timer = timer;
        _fingerprintService = fingerprintService;
    }

    public async Task<IOrderedEnumerable<IEvent>> ForAggregate(Guid id, int fromVersion = 0) 
        => await _timer.Time("IEventStream.ForAggregate", 
            () => _stream
                .Where(e => e.AggregateId == id && e.Version > fromVersion)
                .OrderBy(e => e.Version));

    public async Task<IEnumerable<IEvent>> Where(Func<IEvent, bool> query)
        => await _timer.Time("IEventStream.Where", () => _stream.Where(query));

    public async Task Push<T>(Guid aggregateId, EventType type, T details)
    {
        await _timer.Time($"IEventStream.Push<{typeof(T).Name}>", async () =>
        {
            var existing = await ForAggregate(aggregateId).ToListAsync();
            var maxVersion = existing.Any() ? existing.Max(e => e.Version) : 0;
            var newVersion = maxVersion + 1;
            var fingerprint = await _fingerprintService.Calculate(details);
            _idSerial += 1;
            _stream.Add(Event.Create()
                .For(type)
                .ForAggregate(aggregateId)
                .WithVersion(newVersion)
                .WithDetails(details)
                .WithFingerprint(fingerprint)
                .WithId(_idSerial));
        });
    }
}