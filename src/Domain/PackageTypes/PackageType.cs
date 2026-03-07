namespace Domain.PackageTypes;

public class PackageType
{
    public PackageTypeId Id { get; private set; }
    public LocalizedString Title { get; set; }
    public string ImageIconUrl { get; set; }

    private PackageType(PackageTypeId id, LocalizedString title, string imageIconUrl)
    {
        Id = id;
        Title = title;
        ImageIconUrl = imageIconUrl;
    }

    public static PackageType New(LocalizedString title, string imageIconUrl) =>
        new(PackageTypeId.New(), title, imageIconUrl);

    public void Update(LocalizedString title, string imageIconUrl)
    {
        Title = title;
        ImageIconUrl = imageIconUrl;
    }
}
