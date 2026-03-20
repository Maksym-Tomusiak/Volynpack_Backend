using Application.Common.Interfaces.Queries;
using Domain.ProductPhotos;
using Domain.ProductVariants;

namespace Application.ProductPhotos.Queries;

public record GetProductPhotosByVariantIdQuery(Guid VariantId);

public static class GetProductPhotosByVariantIdQueryHandler
{
    public static async Task<IReadOnlyList<ProductPhoto>> Handle(
        GetProductPhotosByVariantIdQuery query,
        IProductPhotoQueries photoQueries,
        CancellationToken cancellationToken)
    {
        var variantId = new ProductVariantId(query.VariantId);
        return await photoQueries.GetByVariantId(variantId, cancellationToken);
    }
}