using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Orders.Exceptions;
using Domain.Orders;
using LanguageExt;

namespace Application.Orders.Commands;

public record DeleteOrderCommand(Guid Id);

public static class DeleteOrderCommandHandler
{
    public static async Task<Either<OrderException, Order>> Handle(
        DeleteOrderCommand command,
        IOrderRepository orderRepository,
        IOrderQueries orderQueries,
        CancellationToken cancellationToken)
    {
        var orderId = new OrderId(command.Id);
        var existingOption = await orderQueries.GetById(orderId, cancellationToken);

        if (existingOption.IsNone)
            return new OrderNotFoundException(command.Id);

        try
        {
            var order = existingOption.IfNoneUnsafe(() => null!);
            var result = await orderRepository.Delete(order, cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            return new OrderUnknownException(command.Id, ex);
        }
    }
}