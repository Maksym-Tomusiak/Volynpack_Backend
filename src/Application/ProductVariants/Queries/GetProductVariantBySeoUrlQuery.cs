using Application.Common.Interfaces.Queries;
using Application.ProductVariants.Exceptions;
using LanguageExt;

namespace Application.ProductVariants.Queries;

public record GetProductVariantBySeoUrlQuery(string SeoUrl);

public static class GetProductVariantBySeoUrlQueryHandler
{
    public static async Task<Either<ProductVariantException, Domain.ProductVariants.ProductVariant>> Handle(
        GetProductVariantBySeoUrlQuery query,
        IProductVariantQueries variantQueries,
        CancellationToken cancellationToken)
    {
        var result = await variantQueries.GetBySeoUrl(query.SeoUrl, cancellationToken);
        return result.Match<Either<ProductVariantException, Domain.ProductVariants.ProductVariant>>(
            variant => variant,
            () => new ProductVariantNotFoundException(Guid.Empty));
    }
}
