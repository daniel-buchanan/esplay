using esplay.persistence;

namespace esplay.aggregates;

public class Animal : Aggregate<Animal>
{
    public string? EarTag { get; set; }
    public string? RfidTag { get; set; }
    public string Sex { get; set; } = "U";

    protected override void PopulateKeyProperties()
    {
        base.PopulateKeyProperties();
        AddKeyProperty(a => a.EarTag);
        AddKeyProperty(a => a.Sex);
    }
}