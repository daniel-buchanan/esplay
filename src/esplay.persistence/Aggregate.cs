using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json;

namespace esplay.persistence;

public abstract class Aggregate<T> : Aggregate, IAggregate<T> 
    where T : IAggregate
{
    private readonly List<Expression<Func<T, object>>> _keyProperties = new();

    [JsonIgnore]
    public IEnumerable<Expression<Func<T, object>>> KeyProperties
    {
        get
        {
            if (_keyProperties.Any()) return _keyProperties;
            PopulateKeyProperties();
            return _keyProperties;
        }
    }

    protected virtual void PopulateKeyProperties() => AddKeyProperty(a => a.Id);

    protected void AddKeyProperty(Expression<Func<T, object>> prop) 
        => _keyProperties.Add(prop);

    public Task<bool> Mutate(T incoming)
    {
        if (incoming.Version < this.Version)
            return Task.FromResult(false);

        var properties = GetProperties();
        var changed = false;
        foreach (var p in properties)
            changed |= SetPropertyValue(p, incoming);

        SetVersion(incoming.Version);
        
        return Task.FromResult(changed);
    }

    private IEnumerable<PropertyInfo> GetProperties()
    {
        var type = typeof(T);
        return type.GetProperties(
            BindingFlags.Instance |
                     BindingFlags.Public);
    }

    private bool SetPropertyValue(PropertyInfo property, T incoming)
    {
        if (!property.CanWrite) return false;
        var incomingValue = property.GetValue(incoming);
        var existingValue = property.GetValue(this);
        if (existingValue == incomingValue) return false;
        property.SetValue(this, incomingValue);
        return true;
    }
}

public abstract class Aggregate : IAggregate
{
    public Guid Id { get; private set; }
    public int Version { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    
    public IAggregate WithId(Guid id)
    {
        if (Id == Guid.Empty) Id = id;
        return this;
    }

    public IAggregate WithVersion(int version)
    {
        if (Version == 0) SetVersion(version);
        return this;
    }

    protected void SetVersion(int version)
        => Version = version;

    public IAggregate WithCreatedAt(DateTimeOffset date)
    {
        if (CreatedAt == DateTimeOffset.MinValue)
            CreatedAt = date;
        return this;
    }

    public IAggregate WithUpdatedAt(DateTimeOffset date)
    {
        if (UpdatedAt == DateTimeOffset.MinValue)
            UpdatedAt = date;
        return this;
    }
}