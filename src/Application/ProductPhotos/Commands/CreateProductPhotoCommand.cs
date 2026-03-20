using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.ProductPhotos.Exceptions;
using Domain.ProductPhotos;
using Domain.ProductVariants;
using LanguageExt;
using Microsoft.AspNetCore.Http;

namespace Application.ProductPhotos.Commands;

public record CreateProductPhotoCommand(
    Guid ProductVariantId,
    IFormFile Photo,
    bool IsPrimary);

public static class CreateProductPhotoCommandHandler
{
    public static async Task<Either<ProductPhotoException, ProductPhoto>> Handle(
        CreateProductPhotoCommand command,
        IProductPhotoRepository photoRepository,
        IProductVariantQueries variantQueries,
        IFileService fileService,
        CancellationToken cancellationToken)
    {
        var variantId = new ProductVariantId(command.ProductVariantId);
        
        // Перевіряємо, чи існує така варіація товару
        var variantOption = await variantQueries.GetById(variantId, cancellationToken);
        if (variantOption.IsNone)
            return new ProductVariantNotFoundException(command.ProductVariantId);

        try
        {
            // Зберігаємо фізичний файл
            var fileName = await fileService.SaveFileAsync(command.Photo, cancellationToken);
            const string requestPath = "/uploads"; // Або твій шлях
            var photoUrl = $"{requestPath}/{fileName}";

            var photo = ProductPhoto.New(variantId, photoUrl, command.IsPrimary);

            var result = await photoRepository.Add(photo, cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            return new ProductPhotoUnknownException(Guid.Empty, ex);
        }
    }
}