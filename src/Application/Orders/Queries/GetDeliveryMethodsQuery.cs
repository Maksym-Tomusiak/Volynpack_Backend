using Application.Common.Interfaces.Queries;
using Domain.DelivaryMethods;

namespace Application.Orders.Queries;

public record GetDeliveryMethodsQuery();

public static class GetDeliveryMethodsQueryHandler
{
    public static async Task<IReadOnlyList<DeliveryMethod>> Handle(
        GetDeliveryMethodsQuery query,
        IDeliveryMethodQueries queries,
        CancellationToken cancellationToken)
    {
        return await queries.GetAll(cancellationToken);
    }
}
