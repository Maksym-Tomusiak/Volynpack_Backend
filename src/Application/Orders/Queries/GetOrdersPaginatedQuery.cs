using Application.Common.Interfaces.Queries;
using Application.Common.Models;
using Domain.Orders;

namespace Application.Orders.Queries;

public record GetOrdersPaginatedQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    string? SortBy = null,
    bool SortDescending = false);

public static class GetOrdersPaginatedQueryHandler
{
    public static async Task<PaginatedResult<Order>> Handle(
        GetOrdersPaginatedQuery query,
        IOrderQueries orderQueries,
        CancellationToken cancellationToken)
    {
        var parameters = new PaginationParameters(
            query.PageNumber,
            query.PageSize,
            query.SearchTerm,
            query.SortBy,
            query.SortDescending);

        return await orderQueries.GetPaginated(parameters, cancellationToken);
    }
}