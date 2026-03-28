namespace Domain.ProductVariants;

public record ProductVariantId(Guid Value)
{
    public static ProductVariantId Empty() => new(Guid.Empty);
    public static ProductVariantId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}