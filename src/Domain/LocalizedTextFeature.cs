namespace Domain;

public class LocalizedTextFeature(LocalizedString title, LocalizedString description)
{
    public LocalizedString Title { get; private set; } = title;
    public LocalizedString Description { get; private set; } = description;
}