using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Orders.Exceptions;
using Domain.DelivaryMethods;
using Domain.Orders;
using Domain.OrderStatuses;
using Domain.PrintingOptions;
using Domain.ProductVariants;
using LanguageExt;

namespace Application.Orders.Commands;

public record CreateOrderItemCommand(
    Guid ProductVariantId, 
    int Quantity, 
    Guid PrintingOptionId);

public record CreateOrderCommand(
    IEnumerable<CreateOrderItemCommand> Items, 
    Guid DeliveryMethodId,
    string FullName,
    string PhoneNumber,
    string? Town,
    string? Branch);

public static class CreateOrderCommandHandler
{
    public static async Task<Either<OrderException, Order>> Handle(
        CreateOrderCommand command,
        IOrderRepository orderRepository,
        IOrderStatusQueries orderStatusQueries,
        IPrintingOptionQueries printingOptionQueries,
        CancellationToken cancellationToken)
    {
        // 1. Валідація опцій друку (чи існують такі)
        var printingOptions = await printingOptionQueries.GetAll(cancellationToken);
        var printingOptionIds = printingOptions.Select(x => x.Id.Value).ToHashSet();

        foreach (var item in command.Items)
        {
            if (!printingOptionIds.Contains(item.PrintingOptionId))
            {
                return new PrintingOptionNotFoundException(item.PrintingOptionId);
            }
        }

        var domainItems = command.Items.Select(item => (
            new ProductVariantId(item.ProductVariantId),
            item.Quantity,
            new PrintingOptionId(item.PrintingOptionId)
        )).ToList();
        
        var orderStatus = await orderStatusQueries.GetByName("Нове", cancellationToken);

        if (orderStatus.IsNone)
        {
            return new OrderStatusNotFoundException(Guid.Empty);
        }
        
        try
        {
            var order = Order.New(
                orderStatus.First().Id,
                new DeliveryMethodId(command.DeliveryMethodId),
                command.FullName,
                command.PhoneNumber,
                command.Town,
                command.Branch,
                domainItems);

            var result = await orderRepository.Add(order, cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            return new OrderUnknownException(Guid.Empty, ex);
        }
    }
}
