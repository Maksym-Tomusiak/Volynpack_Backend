using Domain.PrintingOptions;
using Domain.ProductVariants;

namespace Domain.Orders;

public class OrderItem
{
    public OrderItemId Id { get; private set; }
    public OrderId OrderId { get; private set; }
    
    public ProductVariantId ProductVariantId { get; private set; }
    public ProductVariant? ProductVariant { get; private set; }
    
    public int Quantity { get; private set; }

    // Зв'язки з довідниками
    public PrintingOptionId PrintingOptionId { get; private set; }
    public PrintingOption? PrintingOption { get; private set; }

    private OrderItem(
        OrderItemId id,
        OrderId orderId,
        ProductVariantId productVariantId,
        int quantity,
        PrintingOptionId printingOptionId)
    {
        Id = id;
        OrderId = orderId;
        ProductVariantId = productVariantId;
        Quantity = quantity;
        PrintingOptionId = printingOptionId;
    }

    public void ChangeQuantity(int newQuantity)
    {
        Quantity = newQuantity;
    }

    public static OrderItem New(
        OrderId orderId,
        ProductVariantId productVariantId,
        int quantity,
        PrintingOptionId printingOptionId) =>
        new(OrderItemId.New(), orderId, productVariantId, quantity, printingOptionId);
}
