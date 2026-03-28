namespace Domain.News;

public record NewsId(Guid Value)
{
    public static NewsId Empty() => new(Guid.Empty);
    public static NewsId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}