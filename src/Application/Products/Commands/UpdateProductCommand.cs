using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Common.Models;
using Application.Products.Exceptions;
using Domain;
using Domain.PackageTypes;
using Domain.ProductCategories;
using Domain.Products;
using LanguageExt;

namespace Application.Products.Commands;

public record UpdateProductCommand(
    Guid Id,
    Guid TypeId,
    string TitleUk,
    string TitleEn,
    string DescriptionUk,
    string DescriptionEn,
    IReadOnlyList<Guid> CategoryIds,
    IReadOnlyList<TextFeatureDto> SuitableFor,
    IReadOnlyList<TextFeatureDto> GeneralCharacteristics);

public static class UpdateProductCommandHandler
{
    public static async Task<Either<ProductException, Product>> Handle(
        UpdateProductCommand command,
        IProductRepository productRepository,
        IProductQueries productQueries,// Використовуємо репозиторій для трекінгу
        IProductCategoryQueries categoryQueries,
        CancellationToken cancellationToken)
    {
        var productId = new ProductId(command.Id);
        
        // ВИПРАВЛЕНО: Дістаємо товар З ТРЕКІНГОМ!
        var existingOption = await productQueries.GetByIdWithTracking(productId, cancellationToken);

        if (existingOption.IsNone)
            return new ProductNotFoundException(command.Id);

        try
        {
            var product = existingOption.First();

            var title = new LocalizedString(command.TitleUk, command.TitleEn);
            var description = new LocalizedString(command.DescriptionUk, command.DescriptionEn);
            var typeId = new PackageTypeId(command.TypeId);
            
            product.UpdateGeneralInfo(typeId, title, description);

            var suitableFor = command.SuitableFor.Select(f => 
                new LocalizedTextFeature(
                    new LocalizedString(f.TitleUk, f.TitleEn), 
                    new LocalizedString(f.DescriptionUk, f.DescriptionEn))).ToList();

            var generalCharacteristics = command.GeneralCharacteristics.Select(f => 
                new LocalizedTextFeature(
                    new LocalizedString(f.TitleUk, f.TitleEn), 
                    new LocalizedString(f.DescriptionUk, f.DescriptionEn))).ToList();

            // Оскільки product має трекінг, EF Core автоматично видалить старі записи і додасть нові
            product.UpdateFeatures(suitableFor, generalCharacteristics);

            // ВИПРАВЛЕНО: Оновлюємо категорії
            var categoryIds = command.CategoryIds.Select(id => new ProductCategoryId(id)).ToList();
            var categories = await categoryQueries.GetByIds(categoryIds, cancellationToken);
            
            product.UpdateCategories(categories.ToList());

            return await productRepository.Update(product, cancellationToken);
        }
        catch (Exception ex)
        {
            return new ProductUnknownException(command.Id, ex);
        }
    }
}