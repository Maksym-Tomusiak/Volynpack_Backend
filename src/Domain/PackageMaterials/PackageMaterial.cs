namespace Domain.PackageMaterials;

public class PackageMaterial
{
    public PackageMaterialId Id { get; private set; }
    public LocalizedString Title { get; set; }

    private PackageMaterial(PackageMaterialId id, LocalizedString title)
    {
        Id = id;
        Title = title;
    }

    public static PackageMaterial New(LocalizedString title) => new(PackageMaterialId.New(), title);

    public void Update(LocalizedString title) => Title = title;
}
