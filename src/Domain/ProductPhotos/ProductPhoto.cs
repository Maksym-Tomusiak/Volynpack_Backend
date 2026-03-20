using Domain.ProductVariants;

namespace Domain.ProductPhotos;

public class ProductPhoto
{
    public ProductPhotoId Id { get; private set; }
    public ProductVariantId ProductVariantId { get; private set; }
    public string PhotoUrl { get; private set; }
    public bool IsPrimary { get; private set; }

    private ProductPhoto(ProductPhotoId id, ProductVariantId productVariantId, string photoUrl, bool isPrimary)
    {
        Id = id;
        ProductVariantId = productVariantId;
        PhotoUrl = photoUrl;
        IsPrimary = isPrimary;
    }

    public static ProductPhoto New(ProductVariantId productVariantId, string photoUrl, bool isPrimary = false)
    {
        return new ProductPhoto(ProductPhotoId.New(), productVariantId, photoUrl, isPrimary);
    }

    public void SetAsPrimary() => IsPrimary = true;
    public void RemovePrimary() => IsPrimary = false;
}