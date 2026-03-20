using Domain.PackageTypes;
using Domain.ProductCategories;

namespace Domain.Products;

public class Product
{
    public ProductId Id { get; private set; }
    public PackageTypeId PackageTypeId { get; private set; }
    public PackageType? Type { get; private set; }
    public LocalizedString Title { get; private set; }
    public LocalizedString Description { get; private set; }
    
    // JSON поля
    public List<LocalizedTextFeature> SuitableFor { get; private set; } = new();
    public List<LocalizedTextFeature> GeneralCharacteristics { get; private set; } = new();
    public ICollection<ProductCategory> Categories { get; set; } = new List<ProductCategory>();

    private Product(ProductId id, PackageTypeId packageTypeId, LocalizedString title, LocalizedString description)
    {
        Id = id;
        PackageTypeId = packageTypeId;
        Title = title;
        Description = description;
    }

    public static Product New(LocalizedString title, PackageTypeId packageTypeId, LocalizedString description) 
        => new(ProductId.New(), packageTypeId, title, description);

    public void UpdateGeneralInfo(PackageTypeId packageTypeId, LocalizedString title, LocalizedString description)
    {
        PackageTypeId = packageTypeId;
        Title = title;
        Description = description;
    }

    public void UpdateFeatures(List<LocalizedTextFeature> suitableFor, List<LocalizedTextFeature> generalCharacteristics)
    {
        SuitableFor.Clear();
        foreach (var feature in suitableFor)
        {
            SuitableFor.Add(feature);
        }

        GeneralCharacteristics.Clear();
        foreach (var feature in generalCharacteristics)
        {
            GeneralCharacteristics.Add(feature);
        }
    }
    
    public void UpdateCategories(IReadOnlyList<ProductCategory> categories)
    {
        Categories.Clear();
        foreach (var category in categories)
        {
            Categories.Add(category);
        }
    }
}