namespace Domain.NewsCategories;

public record NewsCategoryId(Guid Value)
{
    public static NewsCategoryId Empty() => new(Guid.Empty);
    public static NewsCategoryId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}