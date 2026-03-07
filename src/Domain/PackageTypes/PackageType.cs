namespace Domain.PackageTypes;

public class PackageType
{
    public PackageTypeId Id { get; private set; }
    public LocalizedString Title { get; set; }
    public string ImageIconUrl { get; set; }
    public string ImageOverlayUrl { get; set; }

    private PackageType(PackageTypeId id, LocalizedString title, string imageIconUrl, string imageOverlayUrl)
    {
        Id = id;
        Title = title;
        ImageIconUrl = imageIconUrl;
        ImageOverlayUrl = imageOverlayUrl;
    }

    public static PackageType New(LocalizedString title, string imageIconUrl, string imageOverlayUrl) =>
        new(PackageTypeId.New(), title, imageIconUrl, imageOverlayUrl);

    public void Update(LocalizedString title, string imageIconUrl, string imageOverlayUrl)
    {
        Title = title;
        ImageIconUrl = imageIconUrl;
        ImageOverlayUrl = imageOverlayUrl;
    }
}
