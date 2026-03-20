using Application.Common.Interfaces.Queries;
using Application.Common.Models;
using Domain.Products;

namespace Application.Products.Queries;

public record GetProductPaginatedQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    string? SortBy = null,
    bool SortDescending = false);

public static class GetProductPaginatedQueryHandler
{
    public static async Task<PaginatedResult<Product>> Handle(
        GetProductPaginatedQuery query,
        IProductQueries productQueries,
        CancellationToken cancellationToken)
    {
        var parameters = new PaginationParameters(
            query.PageNumber,
            query.PageSize,
            query.SearchTerm,
            query.SortBy,
            query.SortDescending);

        return await productQueries.GetPaginated(parameters, cancellationToken);
    }
}