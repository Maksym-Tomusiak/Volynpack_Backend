using Application.Common.Interfaces.Queries;
using Application.Common.Models;
using Domain.ProductCategories;

namespace Application.ProductCategories.Queries;

public record GetProductCategoryPaginatedQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    string? SortBy = null,
    bool SortDescending = false);

public static class GetProductCategoryPaginatedQueryHandler
{
    public static async Task<PaginatedResult<ProductCategory>> Handle(
        GetProductCategoryPaginatedQuery query,
        IProductCategoryQueries queries,
        CancellationToken cancellationToken)
    {
        var parameters = new PaginationParameters(
            query.PageNumber,
            query.PageSize,
            query.SearchTerm,
            query.SortBy,
            query.SortDescending);

        return await queries.GetPaginated(parameters, cancellationToken);
    }
}