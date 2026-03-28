using Api.Dtos.ProductCategories;
using Domain;
using Domain.Products;

namespace Api.Dtos.Products;

public record LocalizedTextFeatureDto(LocalizedStringDto Title, LocalizedStringDto Description)
{
    public static LocalizedTextFeatureDto FromDomainModel(LocalizedTextFeature feature) =>
        new(new LocalizedStringDto(feature.Title.Uk, feature.Title.En),
            new LocalizedStringDto(feature.Description.Uk, feature.Description.En));
}

public record ProductDto(
    Guid Id,
    Guid TypeId,
    LocalizedStringDto Title,
    LocalizedStringDto Description,
    IReadOnlyList<ProductCategoryDto> Categories,
    // Якщо ти додав колекцію категорій до Product, можна додати сюди IReadOnlyList<ProductCategoryDto> Categories
    IReadOnlyList<LocalizedTextFeatureDto> SuitableFor,
    IReadOnlyList<LocalizedTextFeatureDto> GeneralCharacteristics)
{
    public static ProductDto FromDomainModel(Product product) =>
        new(
            product.Id.Value,
            product.PackageTypeId.Value, // Або TypeId.Value
            new LocalizedStringDto(product.Title.Uk, product.Title.En),
            new LocalizedStringDto(product.Description.Uk, product.Description.En),
            
            // МАПИМО КАТЕГОРІЇ
            product.Categories.Select(ProductCategoryDto.FromDomainModel).ToList(),
            
            product.SuitableFor.Select(LocalizedTextFeatureDto.FromDomainModel).ToList(),
            product.GeneralCharacteristics.Select(LocalizedTextFeatureDto.FromDomainModel).ToList()
        );
}

// Модель для прийому характеристик з фронтенду
public record ProductTextFeatureInputDto(
    string TitleUk, 
    string TitleEn, 
    string DescriptionUk, 
    string DescriptionEn);

public record ProductCreateDto(
    Guid TypeId,
    string TitleUk,
    string TitleEn,
    string DescriptionUk,
    string DescriptionEn,
    IReadOnlyList<Guid> CategoryIds,
    IReadOnlyList<ProductTextFeatureInputDto> SuitableFor,
    IReadOnlyList<ProductTextFeatureInputDto> GeneralCharacteristics);

public record ProductUpdateDto(
    Guid TypeId,
    string TitleUk,
    string TitleEn,
    string DescriptionUk,
    string DescriptionEn,
    IReadOnlyList<Guid> CategoryIds,
    IReadOnlyList<ProductTextFeatureInputDto> SuitableFor,
    IReadOnlyList<ProductTextFeatureInputDto> GeneralCharacteristics);