using Application.Common.Models;
using Domain.ProductPhotos;
using Domain.ProductVariants;
using LanguageExt;

namespace Application.Common.Interfaces.Queries;

public interface IProductPhotoQueries
{
    Task<Option<ProductPhoto>> GetById(ProductPhotoId id, CancellationToken cancellationToken);
    Task<PaginatedResult<ProductPhoto>> GetPaginated(PaginationParameters parameters, CancellationToken cancellationToken);
    
    Task<IReadOnlyList<ProductPhoto>> GetByVariantId(ProductVariantId variantId, CancellationToken cancellationToken);
}