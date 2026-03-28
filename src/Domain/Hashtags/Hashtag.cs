namespace Domain.Hashtags;

public class Hashtag
{
    public HashtagId Id { get; private set; }
    public LocalizedString Name { get; set; }

    private Hashtag(HashtagId id, LocalizedString name)
    {
        Id = id;
        Name = name;
    }

    public static Hashtag New(LocalizedString name) => new(HashtagId.New(), name);
}