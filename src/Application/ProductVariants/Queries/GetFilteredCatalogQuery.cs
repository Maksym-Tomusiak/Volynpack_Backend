using Application.Common.Interfaces.Queries;
using Application.Common.Models;

namespace Application.ProductVariants.Queries;

public record GetFilteredCatalogQuery(ProductFilterParameters Parameters);

public static class GetFilteredCatalogQueryHandler
{
    public static async Task<CatalogFilterResult> Handle(
        GetFilteredCatalogQuery query,
        IProductVariantQueries variantQueries,
        CancellationToken cancellationToken)
    {
        // Вся логіка фасетного пошуку (фільтрація, сортування, групування унікальних товарів) 
        // інкапсульована в репозиторії бази даних для максимальної продуктивності.
        return await variantQueries.GetFilteredCatalog(query.Parameters, cancellationToken);
    }
}