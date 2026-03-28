using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.ProductPhotos.Exceptions;
using Domain.ProductPhotos;
using LanguageExt;

namespace Application.ProductPhotos.Commands;

public record DeleteProductPhotoCommand(Guid Id);

public static class DeleteProductPhotoCommandHandler
{
    public static async Task<Either<ProductPhotoException, ProductPhoto>> Handle(
        DeleteProductPhotoCommand command,
        IProductPhotoRepository photoRepository,
        IProductPhotoQueries photoQueries,
        IFileService fileService,
        CancellationToken cancellationToken)
    {
        var photoId = new ProductPhotoId(command.Id);
        var existingOption = await photoQueries.GetById(photoId, cancellationToken);

        try
        {
            return await existingOption.Match<Task<Either<ProductPhotoException, ProductPhoto>>>(
                async photo =>
                {
                    // Видаляємо фізичний файл
                    if (!string.IsNullOrEmpty(photo.PhotoUrl))
                        await fileService.DeleteFileAsync(photo.PhotoUrl, "product-photos", cancellationToken);

                    // Видаляємо з БД
                    var result = await photoRepository.Delete(photo, cancellationToken);
                    return result;
                },
                () => Task.FromResult<Either<ProductPhotoException, ProductPhoto>>(new ProductPhotoNotFoundException(command.Id)));
        }
        catch (Exception ex)
        {
            return new ProductPhotoUnknownException(command.Id, ex);
        }
    }
}