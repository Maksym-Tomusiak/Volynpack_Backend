namespace Domain.PackageFittings;

public record PackageFittingId(Guid Value)
{
    public static PackageFittingId Empty() => new(Guid.Empty);
    public static PackageFittingId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}
