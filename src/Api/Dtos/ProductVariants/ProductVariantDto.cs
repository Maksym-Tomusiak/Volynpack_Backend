using Api.Dtos.PackageMaterials;
using Api.Dtos.ProductCategories;
using Api.Dtos.ProductPhotos;
using Api.Dtos.Products;
using Application.Common.Models;
using Domain.ProductVariants;

namespace Api.Dtos.ProductVariants;

public record ProductVariantDto(
    Guid Id,
    Guid ProductId,
    ProductDto? Product,
    // ДОДАНО: Назва базового продукту (щоб не тягнути весь ProductDto)
    LocalizedStringDto Title, 
    Guid PackageMaterialId,
    int Density,
    decimal LoadCapacity,
    LocalizedStringDto SeoUrl,
    LocalizedStringDto Availability,
    decimal Height,
    decimal Width,
    decimal? Depth,
    decimal PricePerPiece,
    int QuantityPerPackage,
    bool IsPopular,
    IReadOnlyList<ProductPhotoDto> Photos,
    // ДОДАНО: Категорії базового продукту
    IReadOnlyList<ProductCategoryDto> Categories,
    PackageMaterialDto? Material) 
{
    public static ProductVariantDto FromDomainModel(ProductVariant variant) =>
        new(
            variant.Id.Value,
            variant.ProductId.Value,
            variant.Product == null ? null : ProductDto.FromDomainModel(variant.Product),
            // Мапимо Title базового товару (якщо він завантажений)
            variant.Product != null ? new LocalizedStringDto(variant.Product.Title.Uk, variant.Product.Title.En) : new LocalizedStringDto("", ""),
            variant.PackageMaterialId.Value,
            variant.Density,
            variant.LoadCapacity,
            new LocalizedStringDto(variant.SeoUrl.Uk, variant.SeoUrl.En),
            new LocalizedStringDto(variant.Availability.Uk, variant.Availability.En),
            variant.Height,
            variant.Width,
            variant.Depth,
            variant.PricePerPiece,
            variant.QuantityPerPackage,
            variant.IsPopular,
            variant.Photos?.Select(ProductPhotoDto.FromDomainModel).ToList() ?? new List<ProductPhotoDto>(),
            // Мапимо Категорії базового товару
            variant.Product?.Categories?.Select(ProductCategoryDto.FromDomainModel).ToList() ?? new List<ProductCategoryDto>(),
            variant.Material != null ? PackageMaterialDto.FromDomainModel(variant.Material) : null
        );
}

public record ProductVariantCreateDto(
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

public record ProductVariantUpdateDto(
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

// ── Catalog Filter Models ───────────────────────────────────────────────────

public record CatalogFacetsDto(
    decimal MinAvailablePrice,
    decimal MaxAvailablePrice,
    IReadOnlyList<decimal> AvailableHeights,
    IReadOnlyList<decimal> AvailableWidths,
    IReadOnlyList<decimal?> AvailableDepths,
    IReadOnlyList<int> AvailableDensities,
    IReadOnlyList<Guid> AvailableMaterialIds,
    IReadOnlyList<Guid> AvailableTypeIds)
{
    public static CatalogFacetsDto FromDomainModel(CatalogFacets facets) =>
        new(facets.MinAvailablePrice,
            facets.MaxAvailablePrice,
            facets.AvailableHeights,
            facets.AvailableWidths,
            facets.AvailableDepths,
            facets.AvailableDensities,
            facets.AvailableMaterialIds,
            facets.AvailableTypeIds);
}

public record CatalogFilterResultDto(
    PaginatedResult<ProductVariantDto> Products,
    CatalogFacetsDto Facets);