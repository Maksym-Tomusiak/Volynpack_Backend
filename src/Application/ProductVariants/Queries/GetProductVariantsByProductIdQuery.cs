using Application.Common.Interfaces.Queries;
using Domain.Products;
using Domain.ProductVariants;

namespace Application.ProductVariants.Queries;

public record GetProductVariantsByProductIdQuery(Guid ProductId);

public static class GetProductVariantsByProductIdQueryHandler
{
    public static async Task<IReadOnlyList<ProductVariant>> Handle(
        GetProductVariantsByProductIdQuery query,
        IProductVariantQueries variantQueries,
        CancellationToken cancellationToken)
    {
        var productId = new ProductId(query.ProductId);
        return await variantQueries.GetByProductId(productId, cancellationToken);
    }
}