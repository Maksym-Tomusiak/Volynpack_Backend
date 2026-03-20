using Domain.Products;

namespace Application.Common.Interfaces.Repositories;

public interface IProductRepository
{
    Task<Product> Add(Product product, CancellationToken cancellationToken);
    Task<Product> Update(Product product, CancellationToken cancellationToken);
    Task<Product> Delete(Product product, CancellationToken cancellationToken);
}