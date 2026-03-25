using Application.Common.Interfaces.Queries;
using Domain.OrderStatuses;

namespace Application.Orders.Queries;

public record GetOrderStatusesQuery();

public static class GetOrderStatusesQueryHandler
{
    public static async Task<IReadOnlyList<OrderStatus>> Handle(
        GetOrderStatusesQuery query,
        IOrderStatusQueries queries,
        CancellationToken cancellationToken)
    {
        return await queries.GetAll(cancellationToken);
    }
}