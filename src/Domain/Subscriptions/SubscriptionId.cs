namespace Domain.Subscriptions;

public record SubscriptionId(Guid Value)
{
    public static SubscriptionId Empty() => new(Guid.Empty);
    public static SubscriptionId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}