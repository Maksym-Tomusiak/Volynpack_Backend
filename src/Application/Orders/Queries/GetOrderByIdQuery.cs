using Application.Common.Interfaces.Queries;
using Application.Orders.Exceptions;
using Domain.Orders;
using LanguageExt;

namespace Application.Orders.Queries;

public record GetOrderByIdQuery(Guid Id);

public static class GetOrderByIdQueryHandler
{
    public static async Task<Either<OrderException, Order>> Handle(
        GetOrderByIdQuery query,
        IOrderQueries orderQueries,
        CancellationToken cancellationToken)
    {
        var orderId = new OrderId(query.Id);
        var result = await orderQueries.GetById(orderId, cancellationToken);
        
        return result.Match<Either<OrderException, Order>>(
            order => order,
            () => new OrderNotFoundException(query.Id));
    }
}