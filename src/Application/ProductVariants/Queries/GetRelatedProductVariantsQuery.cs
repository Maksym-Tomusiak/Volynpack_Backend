using Application.Common.Interfaces.Queries;
using Domain.Products;
using Domain.ProductVariants;

namespace Application.ProductVariants.Queries;

public record GetRelatedProductVariantsQuery(Guid TargetProductId, int Limit = 4);

public static class GetRelatedProductVariantsQueryHandler
{
    public static async Task<IReadOnlyList<ProductVariant>> Handle(
        GetRelatedProductVariantsQuery query,
        IProductVariantQueries variantQueries,
        CancellationToken cancellationToken)
    {
        var productId = new ProductId(query.TargetProductId);
        return await variantQueries.GetRelated(productId, query.Limit, cancellationToken);
    }
}