using Domain.DelivaryMethods;
using Domain.OrderStatuses;
using Domain.PrintingOptions;
using Domain.ProductVariants;

namespace Domain.Orders;

public class Order
{
    public OrderId Id { get; private set; }
    
    public ICollection<OrderItem> Items = new List<OrderItem>();

    public DateTime CreatedAt { get; private set; }
    public OrderStatusId OrderStatusId { get; private set; }
    public OrderStatus? OrderStatus { get; private set; }

    public DeliveryMethodId DeliveryMethodId { get; private set; }
    public DeliveryMethod? DeliveryMethod { get; private set; }

    public string FullName { get; private set; }
    public string PhoneNumber { get; private set; }
    public string? Town { get; private set; }
    public string? Branch { get; private set; }

    private Order(
        OrderId id, 
        OrderStatusId orderStatusId,
        DeliveryMethodId deliveryMethodId,
        string fullName,
        string phoneNumber,
        string? town,
        string? branch)
    {
        Id = id;
        OrderStatusId = orderStatusId;
        DeliveryMethodId = deliveryMethodId;
        FullName = fullName;
        PhoneNumber = phoneNumber;
        Town = town;
        Branch = branch;
        CreatedAt = DateTime.UtcNow;
    }

    public static Order New(
        OrderStatusId orderStatusId,
        DeliveryMethodId deliveryMethodId,
        string fullName,
        string phoneNumber,
        string? town,
        string? branch,
        IEnumerable<(ProductVariantId ProductVariantId, int Quantity, PrintingOptionId PrintingOptionId)> items)
    {
        var order = new Order(OrderId.New(), orderStatusId, deliveryMethodId, fullName, phoneNumber, town, branch);
        
        foreach (var item in items)
        {
            order.Items.Add(OrderItem.New(order.Id, item.ProductVariantId, item.Quantity, item.PrintingOptionId));
        }
        
        return order;
    }

    // Зміна статусу адміном
    public void ChangeStatus(OrderStatusId newStatusId)
    {
        OrderStatusId = newStatusId;
    }

    public void UpdateItems(IEnumerable<(ProductVariantId ProductVariantId, int Quantity, PrintingOptionId PrintingOptionId)> newItems)
    {
        Items.Clear();
        foreach (var item in newItems)
        {
            Items.Add(OrderItem.New(Id, item.ProductVariantId, item.Quantity, item.PrintingOptionId));
        }
    }

    public void UpdateDeliveryInfo(
        DeliveryMethodId deliveryMethodId,
        string fullName,
        string phoneNumber,
        string? town,
        string? branch)
    {
        DeliveryMethodId = deliveryMethodId;
        FullName = fullName;
        PhoneNumber = phoneNumber;
        Town = town;
        Branch = branch;
    }
}
