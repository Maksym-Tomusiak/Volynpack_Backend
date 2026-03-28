using Application.Common.Interfaces.Queries;
using Application.ProductVariants.Exceptions;
using Domain.ProductVariants;
using LanguageExt;

namespace Application.ProductVariants.Queries;

public record GetProductVariantByIdQuery(Guid Id);

public static class GetProductVariantByIdQueryHandler
{
    public static async Task<Either<ProductVariantException, ProductVariant>> Handle(
        GetProductVariantByIdQuery query,
        IProductVariantQueries variantQueries,
        CancellationToken cancellationToken)
    {
        var variantId = new ProductVariantId(query.Id);
        var resultOption = await variantQueries.GetById(variantId, cancellationToken);
        
        return resultOption.Match<Either<ProductVariantException, ProductVariant>>(
            variant => variant,
            () => new ProductVariantNotFoundException(query.Id));
    }
}