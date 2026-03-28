namespace Domain.OrderStatuses;

public class OrderStatus
{
    public OrderStatusId Id { get; private set; }
    
    // Тільки для адміна, тому просто string
    public string Name { get; private set; }

    private OrderStatus(OrderStatusId id, string name)
    {
        Id = id;
        Name = name;
    }

    public static OrderStatus New(string name) => new(OrderStatusId.New(), name);
    
    public void Update(string name)
    {
        Name = name;
    }
}