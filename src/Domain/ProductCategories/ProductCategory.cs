namespace Domain.ProductCategories;

public class ProductCategory
{
    public ProductCategoryId Id { get; private set; }
    public LocalizedString Name { get; private set; }

    private ProductCategory(ProductCategoryId id, LocalizedString name)
    {
        Id = id;
        Name = name;
    }

    public static ProductCategory New(LocalizedString name) => new(ProductCategoryId.New(), name);

    public void Update(LocalizedString name) => Name = name;
}