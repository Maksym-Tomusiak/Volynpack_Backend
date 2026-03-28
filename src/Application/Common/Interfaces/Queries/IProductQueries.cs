using Application.Common.Models;
using Domain.Products;
using LanguageExt;

namespace Application.Common.Interfaces.Queries;

public interface IProductQueries
{
    Task<Option<Product>> GetById(ProductId id, CancellationToken cancellationToken);
    Task<Option<Product>> GetByIdWithTracking(ProductId id, CancellationToken cancellationToken);
    Task<PaginatedResult<Product>> GetPaginated(PaginationParameters parameters, CancellationToken cancellationToken);
}