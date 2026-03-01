namespace Domain.NewsSections;

public record NewsSectionId(Guid Value)
{
    public static NewsSectionId Empty() => new(Guid.Empty);
    public static NewsSectionId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}