using Application.Common.Interfaces.Queries;
using Application.ProductPhotos.Exceptions;
using Domain.ProductPhotos;
using LanguageExt;

namespace Application.ProductPhotos.Queries;

public record GetProductPhotoByIdQuery(Guid Id);

public static class GetProductPhotoByIdQueryHandler
{
    public static async Task<Either<ProductPhotoException, ProductPhoto>> Handle(
        GetProductPhotoByIdQuery query,
        IProductPhotoQueries photoQueries,
        CancellationToken cancellationToken)
    {
        var photoId = new ProductPhotoId(query.Id);
        var resultOption = await photoQueries.GetById(photoId, cancellationToken);
        
        return resultOption.Match<Either<ProductPhotoException, ProductPhoto>>(
            photo => photo,
            () => new ProductPhotoNotFoundException(query.Id));
    }
}