using Domain.ProductVariants;

namespace Application.Common.Interfaces.Repositories;

public interface IProductVariantRepository
{
    Task<ProductVariant> Add(ProductVariant variant, CancellationToken cancellationToken);
    Task<ProductVariant> Update(ProductVariant variant, CancellationToken cancellationToken);
    Task<ProductVariant> Delete(ProductVariant variant, CancellationToken cancellationToken);
}