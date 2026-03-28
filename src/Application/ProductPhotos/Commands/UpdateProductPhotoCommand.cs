using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.ProductPhotos.Exceptions;
using Domain.ProductPhotos;
using LanguageExt;

namespace Application.ProductPhotos.Commands;

public record UpdateProductPhotoCommand(
    Guid Id,
    bool IsPrimary);

public static class UpdateProductPhotoCommandHandler
{
    public static async Task<Either<ProductPhotoException, ProductPhoto>> Handle(
        UpdateProductPhotoCommand command,
        IProductPhotoRepository photoRepository,
        IProductPhotoQueries photoQueries,
        CancellationToken cancellationToken)
    {
        var photoId = new ProductPhotoId(command.Id);
        var existingOption = await photoQueries.GetById(photoId, cancellationToken);

        if (existingOption.IsNone)
            return new ProductPhotoNotFoundException(command.Id);

        try
        {
            var photo = existingOption.First();
            
            if (command.IsPrimary)
                photo.SetAsPrimary();
            else
                photo.RemovePrimary();

            var result = await photoRepository.Update(photo, cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            return new ProductPhotoUnknownException(command.Id, ex);
        }
    }
}