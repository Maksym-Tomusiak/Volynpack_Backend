using Domain.PackageMaterials;
using Domain.PackageTypes;
using Domain.ProductPhotos;
using Domain.Products;

namespace Domain.ProductVariants;

public class ProductVariant
{
    public ProductVariantId Id { get; private set; }
    public ProductId ProductId { get; private set; }
    public Product? Product { get; private set; }
    public PackageMaterialId PackageMaterialId { get; private set; }
    public PackageMaterial? Material { get; private set; }
    
    // Оновлені типи
    public int Density { get; private set; }
    public decimal LoadCapacity { get; private set; }
    public LocalizedString SeoUrl { get; private set; } // Додано SeoUrl
    public LocalizedString Availability { get; private set; }
    // Розміри та ціна
    public decimal Height { get; private set; }
    public decimal Width { get; private set; }
    public decimal? Depth { get; private set; }
    public decimal PricePerPiece { get; private set; }
    public int QuantityPerPackage { get; private set; }
    public bool IsPopular { get; private set; }
    public ICollection<ProductPhoto> Photos { get; private set; } = new List<ProductPhoto>();

    private ProductVariant(
        ProductVariantId id, ProductId productId,
        PackageMaterialId packageMaterialId, int density, 
        decimal loadCapacity, LocalizedString seoUrl, LocalizedString availability, decimal height, decimal width, 
        decimal? depth, decimal pricePerPiece, int quantityPerPackage)
    {
        Id = id;
        ProductId = productId;
        PackageMaterialId = packageMaterialId;
        Density = density;
        LoadCapacity = loadCapacity;
        SeoUrl = seoUrl; // Ініціалізація
        Availability = availability;
        Height = height;
        Width = width;
        Depth = depth;
        PricePerPiece = pricePerPiece;
        QuantityPerPackage = quantityPerPackage;
    }

    public static ProductVariant New(
        ProductId productId, PackageMaterialId packageMaterialId, 
        int density, decimal loadCapacity, LocalizedString seoUrl,
        LocalizedString availability,
        decimal height, decimal width, decimal? depth, 
        decimal pricePerPiece, int quantityPerPackage)
    {
        return new ProductVariant(ProductVariantId.New(), productId, 
            packageMaterialId, density, loadCapacity, seoUrl, availability, height, width, 
            depth, pricePerPiece, quantityPerPackage);
    }

    public void Update(
        PackageMaterialId packageMaterialId, 
        int density, decimal loadCapacity, LocalizedString seoUrl,
        LocalizedString availability,
        decimal height, decimal width, decimal? depth, 
        decimal pricePerPiece, int quantityPerPackage)
    {
        PackageMaterialId = packageMaterialId;
        Density = density;
        LoadCapacity = loadCapacity;
        SeoUrl = seoUrl; // Оновлення
        Availability = availability;
        Height = height;
        Width = width;
        Depth = depth;
        PricePerPiece = pricePerPiece;
        QuantityPerPackage = quantityPerPackage;
    }
    
    public void SetPopularStatus(bool isPopular) => IsPopular = isPopular;
}