using Domain.ProductCategories;

namespace Application.Common.Interfaces.Repositories;

public interface IProductCategoryRepository
{
    Task<ProductCategory> Add(ProductCategory category, CancellationToken cancellationToken);
    Task<ProductCategory> Update(ProductCategory category, CancellationToken cancellationToken);
    Task<ProductCategory> Delete(ProductCategory category, CancellationToken cancellationToken);
}