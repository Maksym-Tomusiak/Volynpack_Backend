using Application.Common.Interfaces.Queries;
using Application.Common.Models;
using Domain.ProductVariants;

namespace Application.ProductVariants.Queries;

public record GetPopularProductVariantsPaginatedQuery(
    int PageNumber = 1,
    int PageSize = 10);

public static class GetPopularProductVariantsPaginatedQueryHandler
{
    public static async Task<PaginatedResult<ProductVariant>> Handle(
        GetPopularProductVariantsPaginatedQuery query,
        IProductVariantQueries variantQueries,
        CancellationToken cancellationToken)
    {
        var parameters = new PaginationParameters(query.PageNumber, query.PageSize);
        return await variantQueries.GetPopularPaginated(parameters, cancellationToken);
    }
}