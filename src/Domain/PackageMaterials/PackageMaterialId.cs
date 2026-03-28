namespace Domain.PackageMaterials;

public record PackageMaterialId(Guid Value)
{
    public static PackageMaterialId Empty() => new(Guid.Empty);
    public static PackageMaterialId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}
