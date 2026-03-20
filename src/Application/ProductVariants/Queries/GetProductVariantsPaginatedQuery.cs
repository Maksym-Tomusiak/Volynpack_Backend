using Application.Common.Interfaces.Queries;
using Application.Common.Models;
using Domain.ProductVariants;

namespace Application.ProductVariants.Queries;

public record GetProductVariantsPaginatedQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    string? SortBy = null,
    bool SortDescending = false);

public static class GetProductVariantsPaginatedQueryHandler
{
    public static async Task<PaginatedResult<ProductVariant>> Handle(
        GetProductVariantsPaginatedQuery query,
        IProductVariantQueries variantQueries,
        CancellationToken cancellationToken)
    {
        var parameters = new PaginationParameters(
            query.PageNumber, query.PageSize, query.SearchTerm, query.SortBy, query.SortDescending);

        return await variantQueries.GetPaginated(parameters, cancellationToken);
    }
}