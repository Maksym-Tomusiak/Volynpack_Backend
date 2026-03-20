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

public record CreateProductCommand(
    Guid TypeId,
    string TitleUk,
    string TitleEn,
    string DescriptionUk,
    string DescriptionEn,
    IReadOnlyList<Guid> CategoryIds,
    IReadOnlyList<TextFeatureDto> SuitableFor,
    IReadOnlyList<TextFeatureDto> GeneralCharacteristics);

public static class CreateProductCommandHandler
{
    public static async Task<Either<ProductException, Product>> Handle(
        CreateProductCommand command,
        IProductRepository productRepository,
        IProductCategoryQueries categoryQueries, 
        CancellationToken cancellationToken)
    {
        try
        {
            var title = new LocalizedString(command.TitleUk, command.TitleEn);
            var description = new LocalizedString(command.DescriptionUk, command.DescriptionEn);
            var typeId = new PackageTypeId(command.TypeId);
            
            var product = Product.New(title, typeId, description);

            var suitableFor = command.SuitableFor.Select(f => 
                new LocalizedTextFeature(
                    new LocalizedString(f.TitleUk, f.TitleEn), 
                    new LocalizedString(f.DescriptionUk, f.DescriptionEn))).ToList();

            var generalCharacteristics = command.GeneralCharacteristics.Select(f => 
                new LocalizedTextFeature(
                    new LocalizedString(f.TitleUk, f.TitleEn), 
                    new LocalizedString(f.DescriptionUk, f.DescriptionEn))).ToList();

            product.UpdateFeatures(suitableFor, generalCharacteristics);

            // ВИПРАВЛЕНО: Зберігаємо категорії
            if (command.CategoryIds.Any())
            {
                var categoryIds = command.CategoryIds.Select(id => new ProductCategoryId(id)).ToList();
                var categories = await categoryQueries.GetByIds(categoryIds, cancellationToken);
                
                product.UpdateCategories(categories.ToList()); // Переконайся, що в Product є цей метод
            }

            return await productRepository.Add(product, cancellationToken);
        }
        catch (Exception ex)
        {
            return new ProductUnknownException(Guid.Empty, ex);
        }
    }
}