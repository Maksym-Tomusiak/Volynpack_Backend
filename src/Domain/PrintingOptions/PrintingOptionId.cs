namespace Domain.PrintingOptions;

public record PrintingOptionId(Guid Value)
{
    public static PrintingOptionId Empty() => new(Guid.Empty);
    public static PrintingOptionId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}