namespace Domain.ProductCategories;

public record ProductCategoryId(Guid Value)
{
    public static ProductCategoryId Empty() => new(Guid.Empty);
    public static ProductCategoryId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}