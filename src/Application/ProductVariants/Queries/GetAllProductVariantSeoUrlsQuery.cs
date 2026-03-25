using Application.Common.Interfaces.Queries;
using Domain;

namespace Application.ProductVariants.Queries;

public record GetAllProductVariantSeoUrlsQuery();

public static class GetAllProductVariantSeoUrlsQueryHandler
{
    public static async Task<IReadOnlyList<LocalizedString>> Handle(
        GetAllProductVariantSeoUrlsQuery query,
        IProductVariantQueries queries,
        CancellationToken cancellationToken)
    {
        return await queries.GetAllSeoUrls(cancellationToken);
    }
}
