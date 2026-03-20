using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.ProductVariants.Exceptions;
using Domain;
using Domain.PackageMaterials;
using Domain.PackageTypes;
using Domain.Products;
using Domain.ProductVariants;
using LanguageExt;

namespace Application.ProductVariants.Commands;

public record CreateProductVariantCommand(
    Guid ProductId,
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

public static class CreateProductVariantCommandHandler
{
    public static async Task<Either<ProductVariantException, ProductVariant>> Handle(
        CreateProductVariantCommand command,
        IProductVariantRepository variantRepository,
        IProductQueries productQueries,
        IPackageMaterialQueries materialQueries,
        // IPackagingTypeQueries packagingTypeQueries, // Розікоментуй, якщо є інтерфейс
        CancellationToken cancellationToken)
    {
        var productId = new ProductId(command.ProductId);
        var materialId = new PackageMaterialId(command.PackageMaterialId);

        // Перевіряємо, чи існують базові сутності
        var productOption = await productQueries.GetById(productId, cancellationToken);
        if (productOption.IsNone)
            return new ProductVariantDependencyNotFoundException(command.ProductId, "Product");

        var materialOption = await materialQueries.GetById(materialId, cancellationToken);
        if (materialOption.IsNone)
            return new ProductVariantDependencyNotFoundException(command.PackageMaterialId, "PackageMaterial");

        try
        {
            var seoUrl = new LocalizedString(command.SeoUrlUk, command.SeoUrlEn);
            var availability = new LocalizedString(command.AvailabilityUk, command.AvailabilityEn);

            var variant = ProductVariant.New(
                productId, materialId, 
                command.Density, command.LoadCapacity, seoUrl, availability,
                command.Height, command.Width, command.Depth, 
                command.PricePerPiece, command.QuantityPerPackage);

            if (command.IsPopular)
                variant.SetPopularStatus(true);

            var result = await variantRepository.Add(variant, cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            return new ProductVariantUnknownException(Guid.Empty, ex);
        }
    }
}