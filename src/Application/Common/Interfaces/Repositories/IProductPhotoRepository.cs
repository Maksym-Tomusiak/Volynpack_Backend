using Domain.ProductPhotos;

namespace Application.Common.Interfaces.Repositories;

public interface IProductPhotoRepository
{
    Task<ProductPhoto> Add(ProductPhoto photo, CancellationToken cancellationToken);
    Task<ProductPhoto> Update(ProductPhoto photo, CancellationToken cancellationToken);
    Task<ProductPhoto> Delete(ProductPhoto photo, CancellationToken cancellationToken);
}