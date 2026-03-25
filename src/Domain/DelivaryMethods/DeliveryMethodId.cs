namespace Domain.DelivaryMethods;

public record DeliveryMethodId(Guid Value)
{
    public static DeliveryMethodId Empty() => new(Guid.Empty);
    public static DeliveryMethodId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}
