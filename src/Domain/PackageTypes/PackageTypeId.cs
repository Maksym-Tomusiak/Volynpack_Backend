namespace Domain.PackageTypes;

public record PackageTypeId(Guid Value)
{
    public static PackageTypeId Empty() => new(Guid.Empty);
    public static PackageTypeId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}
