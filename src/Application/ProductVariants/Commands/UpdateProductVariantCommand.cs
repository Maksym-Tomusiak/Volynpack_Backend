using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.ProductVariants.Exceptions;
using Domain;
using Domain.PackageMaterials;
using Domain.PackageTypes;
using Domain.ProductVariants;
using LanguageExt;

namespace Application.ProductVariants.Commands;

public record UpdateProductVariantCommand(
    Guid Id,
    Guid PackageMaterialId,
    int Density,
    decimal LoadCapacity,
    string SeoUrlUk,
    string SeoUrlEn,
    string AvailabilityUk,
    string AvailabilityEn,
    decimal Height,
    decimal Width,
    decimal? Depth,
    decimal PricePerPiece,
    int QuantityPerPackage,
    bool IsPopular);

public static class UpdateProductVariantCommandHandler
{
    public static async Task<Either<ProductVariantException, ProductVariant>> Handle(
        UpdateProductVariantCommand command,
        IProductVariantRepository variantRepository,
        IProductVariantQueries variantQueries,
        CancellationToken cancellationToken)
    {
        var variantId = new ProductVariantId(command.Id);
        
        // ВИПРАВЛЕНО: Дістаємо варіацію з трекінгом і БЕЗ базового товару
        var existingOption = await variantQueries.GetByIdWithTracking(variantId, cancellationToken);

        if (existingOption.IsNone)
            return new ProductVariantNotFoundException(command.Id);

        try
        {
            var variant = existingOption.First();
            var materialId = new PackageMaterialId(command.PackageMaterialId);
            var seoUrl = new LocalizedString(command.SeoUrlUk, command.SeoUrlEn);
            var availability = new LocalizedString(command.AvailabilityUk, command.AvailabilityEn);

            // Оновлюємо поля варіації
            variant.Update(
                materialId, 
                command.Density, 
                command.LoadCapacity, 
                seoUrl, 
                availability,
                command.Height, 
                command.Width, 
                command.Depth, 
                command.PricePerPiece, 
                command.QuantityPerPackage);

            variant.SetPopularStatus(command.IsPopular);

            // Зберігаємо зміни
            return await variantRepository.Update(variant, cancellationToken);
        }
        catch (Exception ex)
        {
            return new ProductVariantUnknownException(command.Id, ex);
        }
    }
}