namespace Domain.OrderStatuses;

public record OrderStatusId(Guid Value)
{
    public static OrderStatusId Empty() => new(Guid.Empty);
    public static OrderStatusId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}