using Application.Common.Models;
using Domain.Products;
using Domain.ProductVariants;
using LanguageExt;

namespace Application.Common.Interfaces.Queries;

public interface IProductVariantQueries
{
    Task<Option<ProductVariant>> GetById(ProductVariantId id, CancellationToken cancellationToken);
    Task<PaginatedResult<ProductVariant>> GetPaginated(PaginationParameters parameters, CancellationToken cancellationToken);
    Task<IReadOnlyList<ProductVariant>> GetByProductId(ProductId productId, CancellationToken cancellationToken);
    Task<CatalogFilterResult> GetFilteredCatalog(ProductFilterParameters parameters, CancellationToken cancellationToken);
    Task<PaginatedResult<ProductVariant>> GetPopularPaginated(PaginationParameters parameters, CancellationToken cancellationToken);
    Task<IReadOnlyList<ProductVariant>> GetRelated(ProductId targetProductId, int limit, CancellationToken cancellationToken);
    Task<Option<ProductVariant>> GetByIdWithTracking(ProductVariantId id, CancellationToken cancellationToken);
    Task<Option<ProductVariant>> GetBySeoUrl(string seoUrl, CancellationToken cancellationToken);
}