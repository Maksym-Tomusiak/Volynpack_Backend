using Application.Common.Models;
using Domain.ProductCategories;
using LanguageExt;

namespace Application.Common.Interfaces.Queries;

public interface IProductCategoryQueries
{
    Task<IReadOnlyList<ProductCategory>> GetAll(CancellationToken cancellationToken);
    Task<Option<ProductCategory>> GetById(ProductCategoryId id, CancellationToken cancellationToken);
    Task<IReadOnlyList<ProductCategory>> GetByIds(IEnumerable<ProductCategoryId> ids, CancellationToken cancellationToken);
    Task<PaginatedResult<ProductCategory>> GetPaginated(PaginationParameters parameters, CancellationToken cancellationToken);
}