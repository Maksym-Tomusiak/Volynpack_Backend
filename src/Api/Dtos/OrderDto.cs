using Domain.DelivaryMethods;
using Domain.OrderStatuses;
using Domain.PrintingOptions;

namespace Api.Dtos;

using Domain.Orders;

// ── OrderStatus ──────────────────────────────────────────────────────────────
public record OrderStatusDto(Guid Id, string Name)
{
    public static OrderStatusDto FromDomainModel(OrderStatus status) =>
        new(status.Id.Value, status.Name);
}

// ── DeliveryMethod ───────────────────────────────────────────────────────────
public record DeliveryMethodDto(Guid Id, LocalizedStringDto Name)
{
    public static DeliveryMethodDto FromDomainModel(DeliveryMethod method) =>
        new(method.Id.Value, new LocalizedStringDto(method.Name.Uk, method.Name.En));
}

// ── PrintingOption ───────────────────────────────────────────────────────────
public record PrintingOptionDto(Guid Id, LocalizedStringDto Name)
{
    public static PrintingOptionDto FromDomainModel(PrintingOption option) =>
        new(option.Id.Value, new LocalizedStringDto(option.Name.Uk, option.Name.En));
}

// ── OrderItem ─────────────────────────────────────────────────────────────
public record OrderItemDto(
    Guid Id,
    Guid ProductVariantId,
    LocalizedStringDto ProductTitle,
    LocalizedStringDto MaterialName,
    int Density,
    decimal Width,
    decimal Height,
    decimal? Depth,
    int Quantity,
    int QuantityPerPackage,
    decimal PricePerPiece,
    PrintingOptionDto PrintingOption)
{
    public static OrderItemDto FromDomainModel(OrderItem item) =>
        new(
            item.Id.Value,
            item.ProductVariantId.Value,
            item.ProductVariant?.Product is null ? null! : new LocalizedStringDto(item.ProductVariant.Product.Title.Uk, item.ProductVariant.Product.Title.En),
            item.ProductVariant?.Material is null ? null! : new LocalizedStringDto(item.ProductVariant.Material.Title.Uk, item.ProductVariant.Material.Title.En),
            item.ProductVariant?.Density ?? 0,
            item.ProductVariant?.Width ?? 0,
            item.ProductVariant?.Height ?? 0,
            item.ProductVariant?.Depth,
            item.Quantity,
            item.ProductVariant?.QuantityPerPackage ?? 1,
            item.ProductVariant?.PricePerPiece ?? 0,
            item.PrintingOption is null ? null! : PrintingOptionDto.FromDomainModel(item.PrintingOption));
}

// ── Order ────────────────────────────────────────────────────────────────────
public record OrderDto(
    Guid Id,
    IEnumerable<OrderItemDto> Items,
    OrderStatusDto OrderStatus,
    DeliveryMethodDto DeliveryMethod,
    string FullName,
    string PhoneNumber,
    string? Town,
    string? Branch,
    DateTime CreatedAt)
{
    public static OrderDto FromDomainModel(Order order) =>
        new(
            order.Id.Value,
            order.Items.Select(OrderItemDto.FromDomainModel),
            order.OrderStatus is null ? null! : OrderStatusDto.FromDomainModel(order.OrderStatus),
            order.DeliveryMethod is null ? null! : DeliveryMethodDto.FromDomainModel(order.DeliveryMethod),
            order.FullName,
            order.PhoneNumber,
            order.Town,
            order.Branch,
            order.CreatedAt);
}

// ── Crate & Update ───────────────────────────────────────────────────────────

public record CreateOrderItemDto(
    Guid ProductVariantId, 
    int Quantity, 
    Guid PrintingOptionId);

public record CreateOrderDto(
    IEnumerable<CreateOrderItemDto> Items, 
    Guid DeliveryMethodId,
    string FullName,
    string PhoneNumber,
    string? Town,
    string? Branch);

public record UpdateOrderDto(
    Guid OrderStatusId,
    Guid DeliveryMethodId,
    string FullName,
    string PhoneNumber,
    string? Town,
    string? Branch,
    IEnumerable<CreateOrderItemDto> Items);
