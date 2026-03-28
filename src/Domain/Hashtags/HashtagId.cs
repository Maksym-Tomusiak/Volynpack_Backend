namespace Domain.Hashtags;

public record HashtagId(Guid Value)
{
    public static HashtagId Empty() => new(Guid.Empty);
    public static HashtagId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}