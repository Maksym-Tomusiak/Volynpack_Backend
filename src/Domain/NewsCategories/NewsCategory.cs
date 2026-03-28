namespace Domain.NewsCategories;

public class NewsCategory
{
    public NewsCategoryId Id { get; private set; }
    public LocalizedString Title { get; set; }

    private NewsCategory(NewsCategoryId id, LocalizedString title)
    {
        Id = id;
        Title = title;
    }

    public static NewsCategory New(LocalizedString title) => new(NewsCategoryId.New(), title);

    public void Update(LocalizedString title) => Title = title;
}