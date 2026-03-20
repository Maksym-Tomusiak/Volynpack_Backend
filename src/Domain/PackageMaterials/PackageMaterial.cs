namespace Domain.PackageMaterials;

public class PackageMaterial
{
    public PackageMaterialId Id { get; private set; }
    public LocalizedString Title { get; set; }
    public LocalizedString Description { get; set; }

    private PackageMaterial(PackageMaterialId id, LocalizedString title, LocalizedString description)
    {
        Id = id;
        Title = title;
        Description = description;
    }

    public static PackageMaterial New(LocalizedString title, LocalizedString description) => new(PackageMaterialId.New(), title, description);

    public void Update(LocalizedString title, LocalizedString description) => (this.Title, this.Description) = (title, description);
}
