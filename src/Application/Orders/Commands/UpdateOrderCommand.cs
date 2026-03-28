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

public record UpdateOrderCommand(
    Guid OrderId, 
    Guid NewStatusId,
    Guid DeliveryMethodId,
    string FullName,
    string PhoneNumber,
    string? Town,
    string? Branch,
    IEnumerable<(Guid ProductVariantId, int Quantity, Guid PrintingOptionId)> Items);

public static class UpdateOrderCommandHandler
{
    public static async Task<Either<OrderException, Order>> Handle(
        UpdateOrderCommand command,
        IOrderRepository orderRepository,
        IOrderQueries orderQueries,
        IOrderStatusQueries orderStatusQueries,
        CancellationToken cancellationToken)
    {
        var orderId = new OrderId(command.OrderId);
        var existingOption = await orderQueries.GetById(orderId, cancellationToken);
        
        if (existingOption.IsNone)
            return new OrderNotFoundException(command.OrderId);

        // Перевіряємо, чи існує такий статус
        var statuses = await orderStatusQueries.GetAll(cancellationToken);
        if (!statuses.Any(x => x.Id.Value == command.NewStatusId))
        {
            return new OrderStatusNotFoundException(command.NewStatusId);
        }

        try
        {
            var order = existingOption.IfNoneUnsafe(() => null!);
            
            order.ChangeStatus(new OrderStatusId(command.NewStatusId));
            order.UpdateDeliveryInfo(
                new DeliveryMethodId(command.DeliveryMethodId),
                command.FullName,
                command.PhoneNumber,
                command.Town,
                command.Branch);
            
            order.UpdateItems(command.Items.Select(i => (
                new ProductVariantId(i.ProductVariantId),
                i.Quantity,
                new PrintingOptionId(i.PrintingOptionId))));

            var result = await orderRepository.Update(order, cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            return new OrderUnknownException(command.OrderId, ex);
        }
    }
}
